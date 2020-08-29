using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;

namespace GamemodesClient.Core
{
    public struct GmNetEntity<T> where T : Entity
    {
        private int m_handle;
        private bool m_isNetworked;
        private T m_cachedEntity;
        public T Entity
        {
            get
            {
                if (m_cachedEntity == null || !m_cachedEntity.Exists())
                {
                    int handle = m_isNetworked ? API.NetworkGetEntityFromNetworkId(m_handle) : m_handle;

                    switch (API.GetEntityType(handle))
                    {
                        case 1:
                            m_cachedEntity = new Ped(handle) as T;

                            break;
                        case 2:
                            m_cachedEntity = new Vehicle(handle) as T;

                            break;
                        case 3:
                            m_cachedEntity = new Prop(handle) as T;

                            break;
                        default:
                            m_cachedEntity = null;

                            break;
                    }
                }

                return m_cachedEntity;
            }
        }
        public bool Exists
        {
            get
            {
                return m_handle != default && API.NetworkGetEntityFromNetworkId(m_handle) != 0;
            }
        }

        public GmNetEntity(int _handle, bool _isNetworked)
        {
            m_handle = _handle;
            m_isNetworked = _isNetworked;
            m_cachedEntity = null;
        }

        public GmNetEntity(Entity _entity)
        {
            m_handle = _entity.IsNetworked() ? _entity.NetworkId : _entity.Handle;
            m_isNetworked = _entity.IsNetworked();
            m_cachedEntity = null;
        }
    }
}
