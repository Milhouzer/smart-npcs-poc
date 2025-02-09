#pragma once

#include "CoreMinimal.h"
#include "Game/LevelSimulation/Components/SimulationRenderer.h"
#include "GameFramework/Actor.h"
#include "CaravanActor.generated.h"

UCLASS()
class SNP_API ACaravanActor : public AActor
{
	GENERATED_BODY()

public:
	ACaravanActor();

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	USimulationRenderer* SimRenderer;
	
protected:
	virtual void BeginPlay() override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

};