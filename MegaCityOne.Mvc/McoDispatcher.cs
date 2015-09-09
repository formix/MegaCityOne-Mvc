using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MegaCityOne.Mvc
{
    /// <summary>
    /// The Judge Dispatcher is responsible to check if a Judge is available for 
    /// the call. If no Judge is available, a Judge will be summoned. 
    /// Dispatched Judges must be returned to the pool by using the 
    /// Dispatcher.Return method. Otherwise, the Dispatch method will summon 
    /// a new Judge on each call. The Dispatcher as a JudgePool. This class is 
    /// a singleton and cannot be instanciated. You must use the static 
    /// member Dispatcher.Current to use an instance of this class. This class
    /// is thread safe.
    /// </summary>
    public sealed class McoDispatcher
    {
        #region Events

        /// <summary>
        /// Event fired when there is no Judge available for the current 
        /// thread id. The event handler is expected to create a Judge,
        /// provide it with laws and attach it the the event args.
        /// </summary>
        public event JudgeSummonDelegate Summon;

        #endregion

        #region Fields

        private static McoDispatcher current = null;

        private Stack<Judge> judgePool;
        private HashSet<int> dispatchedJudges;

        #endregion

        #region Properties

        /// <summary>
        /// The static dispatcher instance for the current application.
        /// </summary>
        public static McoDispatcher Current
        {
            get
            {
                if (current == null)
                {
                    current = new McoDispatcher();
                }
                return current;
            }
        }

        /// <summary>
        /// Gets the Principal of the current thread.
        /// </summary>
        public static IPrincipal Principal
        {
            get
            {
                Judge judge = Current.Dispatch();
                IPrincipal principal = judge.Principal;
                Current.Returns(judge);
                return principal;
            }
        }

        #endregion

        #region Constructors

        private McoDispatcher()
        {
            this.judgePool = new Stack<Judge>();
            this.dispatchedJudges = new HashSet<int>();
        }

        #endregion

        #region Methods


        /// <summary>
        /// Dispatch this Advise call to an available Judge in the pool.
        /// </summary>
        /// <param name="law">The law to Advise.</param>
        /// <param name="arguments">Optionnal arguments provided to help the 
        /// judge to give his advice. By default, the last argument is 
        /// always the HttpContext.Current.</param>
        /// <returns>True is the law is respected, false otherwise.</returns>
        public static bool Advise(string law, params object[] arguments)
        {
            IPrincipal oldPrincipal = Thread.CurrentPrincipal;
            if (HttpContext.Current != null)
            {
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }
            
            Judge judge = Current.Dispatch();
            try
            {
                object[] args = AppendHttpContext(arguments);
                return judge.Advise(law, args);
            }
            finally
            {
                Thread.CurrentPrincipal = oldPrincipal;
                Current.Returns(judge);
            }
        }

        /// <summary>
        /// Dispatch this Enforce call to an available Judge in the pool.
        /// </summary>
        /// <param name="law">The law to Enforce.</param>
        /// <param name="arguments">Optionnal arguments provided to help the 
        /// judge to enforce the law. By default, the last argument is 
        /// always the HttpContext.Current.</param>
        public static void Enforce(string law, params object[] arguments)
        {
            IPrincipal oldPrincipal = Thread.CurrentPrincipal;
            if (HttpContext.Current != null)
            {
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }
            
            Judge judge = Current.Dispatch();
            try
            {
                object[] args = AppendHttpContext(arguments);
                judge.Enforce(law, args);
            }
            finally
            {
                Thread.CurrentPrincipal = oldPrincipal;
                Current.Returns(judge);
            }
        }

        private static object[] AppendHttpContext(object[] arguments)
        {
            object[] args = new object[arguments.Length + 1];
            Array.Copy(arguments, 0, args, 0, arguments.Length);
            args[args.Length - 1] = HttpContext.Current;
            return args;
        }

        /// <summary>
        /// Thread safe. Calling the dispatch method can trigger the Summon 
        /// event if there is no Judge available in the pool. If this 
        /// is the case, it is assumed that a Summon event handler will create 
        /// a Judge and asign it to the SummonEventArgs.Respondent property. 
        /// Otherwise, return an existing Judge from the pool.
        /// </summary>
        /// <returns>A Judge available to answer the call.</returns>
        public Judge Dispatch()
        {
            Judge judge = null;
            lock (this.judgePool)
            {
                if (this.judgePool.Count == 0)
                {
                    JudgeSummonEventArgs e = new JudgeSummonEventArgs();
                    this.OnSummon(e);
                    if (e.Respondent == null)
                    {
                        throw new InvalidOperationException("The Judge summoning returned null.");
                    }
                    this.judgePool.Push(e.Respondent);
                }
                judge = this.judgePool.Pop();
                this.dispatchedJudges.Add(judge.GetHashCode());
            }
            return judge;
        }

        /// <summary>
        /// Thread safe. Returns a dispatched judge to the pool. This method 
        /// do not accept a judge that have not been dispatched by the 
        /// current instance of the dispatcher.
        /// </summary>
        /// <param name="judge">The judge that answered a previous call to 
        /// Dispatch.</param>
        public void Returns(Judge judge)
        {
            lock (this.judgePool)
            {
                if (judge == null)
                {
                    throw new ArgumentNullException("judge");
                }

                if (!this.dispatchedJudges.Contains(judge.GetHashCode()))
                {
                    throw new ArgumentException(
                        "The judge received have not been dispatched by an " +
                        "earlier call to Dispatch()");
                }

                this.dispatchedJudges.Remove(judge.GetHashCode());
                this.judgePool.Push(judge);
            }
        }

        /// <summary>
        /// Method used to fire a Summon event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnSummon(JudgeSummonEventArgs e)
        {
            if (this.Summon != null)
            {
                this.Summon(this, e);
            }
        }

        #endregion
    }
}
