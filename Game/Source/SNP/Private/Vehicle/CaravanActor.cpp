#include "Vehicle/CaravanActor.h"

ACaravanActor::ACaravanActor()
{
    bReplicates = true;
    PrimaryActorTick.bCanEverTick = false;
    SimRenderer = CreateDefaultSubobject<USimulationRenderer>("Simulator");
}

void ACaravanActor::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeProps);
}
void ACaravanActor::BeginPlay()
{
     Super::BeginPlay();
     
     if (!GetWorld())
     {
         UE_LOG(LogTemp, Warning, TEXT("Invalid World"));
         return;
     }

     if (HasAuthority())
     {
         SimRenderer->Init();
     }
}