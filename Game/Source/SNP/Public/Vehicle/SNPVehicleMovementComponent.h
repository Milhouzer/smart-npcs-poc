#pragma once

#include "GameFramework/PawnMovementComponent.h"
#include "SNPVehicleMovementComponent.generated.h"


/**
 * 
 */
UCLASS()
class SNP_API USNPVehicleMovementComponent : public UPawnMovementComponent
{
	GENERATED_BODY()
	
protected:
	virtual void TickComponent(float DeltaTime, enum ELevelTick TickType, FActorComponentTickFunction *ThisTickFunction) override;
};
