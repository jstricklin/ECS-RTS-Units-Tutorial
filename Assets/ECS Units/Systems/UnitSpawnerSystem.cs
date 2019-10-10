// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Collider = Unity.Physics.Collider;

// // JobComponentSystems can run on worker threads.
// // However, creating and removing Entities can only be done on the main thread to prevent race conditions.
// // The system uses an EntityCommandBuffer to defer tasks that can't be done inside the Job.

// // ReSharper disable once InconsistentNaming
// [UpdateInGroup(typeof(SimulationSystemGroup))]
// public class UnitSpawnerSystem : JobComponentSystem
// {
//     // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
//     // when that barrier system executes.
//     // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
//     // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
//     // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
//     // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
//     // // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
//     BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

//     EntityQuery m_MainGroup;

//     protected override void OnCreate()
//     {
//         // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
//         m_MainGroup = GetEntityQuery(ComponentType.ReadOnly<Unity.Physics.PhysicsMass>());
//         m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
//     }

//     struct SpawnJob : IJobForEachWithEntity<UnitSpawner, LocalToWorld>
//     {
//         public EntityCommandBuffer CommandBuffer;

//         public void Execute(Entity entity, int index, [ReadOnly] ref UnitSpawner spawner,
//             [ReadOnly] ref LocalToWorld location)
//         {
//             var entityManager = World.Active.EntityManager;

//             for (var x = 0; x < spawner.CountX; x++)
//             {
//                 for (var y = 0; y < spawner.CountY; y++)
//                 {
//                     // var instance = CommandBuffer.Instantiate(spawner.Prefab);
//                     // BlobAssetReference<Collider> sourceCollider = entityManager.GetComponentData<Unity.Physics.PhysicsCollider>(instance).Value;
//                     // var position = math.transform(location.Value,
//                     //     new float3(x * 2, 0, y * 2));
//                         // entityManager.SetComponentData<Translation>(instance, new Translation {Value = position});
//                     // CommandBuffer.SetComponent(instance, new Translation { Value = position });
//                     // CommandBuffer.AddComponent(index, instance, new PlayerInput());
//                     // CommandBuffer.SetComponent(index, instance, new UnitNavAgent());
//                 }
//             }
//             CommandBuffer.DestroyEntity(entity);
//         }
//     }

//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
//         //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.

//         // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
//         var job = new SpawnJob
//         {
//             CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
//         }.ScheduleSingle(this, inputDeps);

//         // var job = new SpawnJob();
//         // SpawnJob runs in parallel with no sync point until the barrier system executes.
//         // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
//         // We need to tell the barrier system which job it needs to complete before it can play back the commands.
//         // m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

//         return job;
//     }
// }

