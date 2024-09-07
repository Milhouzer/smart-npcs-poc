using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Milhouzer.Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientRequestGameEntrySystem : ISystem
    {
        private EntityQuery _pendingNetworkIdQuery;

        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            _pendingNetworkIdQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate(_pendingNetworkIdQuery);
            state.RequireForUpdate<ClientConnectionRequest>();
        }

        public void OnUpdate(ref SystemState state)
        {
            FixedString32Bytes room = SystemAPI.GetSingleton<ClientConnectionRequest>().Room;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            NativeArray<Entity> pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity pendingNetworkId in pendingNetworkIds)
            {
                ecb.AddComponent<NetworkStreamInGame>(pendingNetworkId);
                Entity requestTeamEntity = ecb.CreateEntity();
                ecb.AddComponent(requestTeamEntity, new RoomConnectionRequest { Room = room });
                ecb.AddComponent(requestTeamEntity, new SendRpcCommandRequest { TargetConnection = pendingNetworkId });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    public struct RoomConnectionRequest : IRpcCommand
    {
        public FixedString32Bytes Room;
    }

}