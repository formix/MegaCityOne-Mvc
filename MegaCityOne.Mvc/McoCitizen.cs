using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;

namespace MegaCityOne.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class McoCitizen
    {

        #region Fields

        private IPrincipal principal;
        private string name;
        private string[] roles;
        private IDictionary<string, object> data;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the current citizen.
        /// </summary>
        public string Name 
        { 
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.principal = null;
                }
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets roles given to the current citizen.
        /// </summary>
        public string[] Roles 
        {
            get
            {
                return this.roles;
            }

            set
            {
                if (!this.AreEquals(this.roles, value))
                {
                    this.principal = null;
                }
                this.roles = value;
            }
        }

        /// <summary>
        /// Gets useful data attached to the current citizen.
        /// </summary>
        public IDictionary<string, object> Data 
        { 
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Gets the security principal corresponding to the current citizen.
        /// </summary>
        [XmlIgnore]
        public IPrincipal Principal
        {
            get
            {
                if (this.principal == null)
                {
                    this.principal = this.CreatePrincipal();
                }
                return this.principal;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of an McoCitizen.
        /// </summary>
        /// <param name="name">The name of the citizen</param>
        /// <param name="roles">Roles given to the citizen</param>
        public McoCitizen(string name, string[] roles)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            this.principal = null;
            this.Name = name;
            this.Roles = roles;
            this.data = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// By default, this virtual method creates A GenericPrincipal with 
        /// the informations available in the current citizen. The 
        /// GenericIdentity.BootstrapContext is set to the current citizen 
        /// for references in laws. Override this method to create an 
        /// IPrincipal corresponding to your application needs.
        /// </summary>
        /// <returns>A security Principal</returns>
        protected virtual IPrincipal CreatePrincipal()
        {
            return new GenericPrincipal(
                new GenericIdentity(this.name) { BootstrapContext = this },
                this.Roles);
        }

        private bool AreEquals(string[] ar1, string[] ar2)
        {
            if (ar1 == null)
            {
                if (ar2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ar2 == null)
                {
                    return false;
                }
            }

            if (ar1.Length != ar2.Length)
            {
                return false;
            }

            for (int i = 0; i < ar1.Length; i++)
            {
                if (ar1[i] != ar2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

        #endregion
}
