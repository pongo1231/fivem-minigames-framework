using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Networked entity helper class
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public struct GmNetEntity<T> where T : Entity
    {
        /// <summary>
        /// Handle (or network id) of entity
        /// </summary>
        private int m_handle;

        /// <summary>
        /// Whether entity is networked or not
        /// </summary>
        private bool m_isNetworked;

        /// <summary>
        /// Cached entity
        /// </summary>
        private T m_cachedEntity;

        /// <summary>
        /// Fetch the entity via handle or network id
        /// </summary>
        public T Entity
        {
            get
            {
                // Check if cached entity doesn't exist
                if (m_cachedEntity == null || !m_cachedEntity.Exists())
                {
                    // Get entity from either handle or network id
                    int handle = m_isNetworked ? API.NetworkGetEntityFromNetworkId(m_handle) : m_handle;

                    // Get entity type
                    switch (API.GetEntityType(handle))
                    {
                        // Ped
                        case 1:
                            m_cachedEntity = new Ped(handle) as T;

                            break;

                        // Vehicle
                        case 2:
                            m_cachedEntity = new Vehicle(handle) as T;

                            break;

                        // Prop
                        case 3:
                            m_cachedEntity = new Prop(handle) as T;

                            break;

                        // Invalid
                        default:
                            m_cachedEntity = null;

                            break;
                    }
                }

                // Return found entity
                return m_cachedEntity;
            }
        }

        /// <summary>
        /// Whether entity exists on this client
        /// </summary>
        public bool Exists
        {
            get
            {
                // Return whether handle or network id is (currently) invalid
                return m_handle != 0 && API.NetworkGetEntityFromNetworkId(m_handle) != 0;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_handle">Handle or network id of entity</param>
        /// <param name="_isNetworked">Whether entity is networked</param>
        public GmNetEntity(int _handle, bool _isNetworked)
        {
            m_handle = _handle;
            m_isNetworked = _isNetworked;
            m_cachedEntity = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_entity">Entity</param>
        public GmNetEntity(Entity _entity)
        {
            m_handle = _entity.IsNetworked() ? _entity.NetworkId : _entity.Handle;
            m_isNetworked = _entity.IsNetworked();
            m_cachedEntity = null;
        }
    }
}
