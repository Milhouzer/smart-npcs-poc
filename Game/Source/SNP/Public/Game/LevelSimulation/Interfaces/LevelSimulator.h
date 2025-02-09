#pragma once

#include "CoreMinimal.h"
#include "SimulatedLevel.h"
#include "UObject/Interface.h"
#include "LevelSimulator.generated.h"

class FLevelSimulation;

UINTERFACE(MinimalAPI)
class ULevelSimulator : public UInterface
{
	GENERATED_BODY()
};

class SNP_API ILevelSimulator
{
	GENERATED_BODY()
	
public:
	virtual void Init() = 0;
	virtual void Render(AActor* Actor) = 0;
	virtual void StopRender(AActor* Actor) = 0;
	virtual TScriptInterface<ISimulatedLevel> GetSimulatedLevel() = 0; 
};

