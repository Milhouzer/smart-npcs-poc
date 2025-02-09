
#pragma once
#include "Vehicle/SNPVehicle.h"
#include "Character/ALSPlayerCameraManager.h"
#include "SNPPlayerCameraManager.generated.h"

/**
 * Player camera manager class
 */
UCLASS(Blueprintable, BlueprintType)
class SNP_API ASNPPlayerCameraManager : public AALSPlayerCameraManager
{
	GENERATED_BODY()
public:
	void OnPossess(ASNPVehicle* NewVehicle);
	
protected:
	virtual void UpdateViewTargetInternal(FTViewTarget& OutVT, float DeltaTime) override;
	
	UFUNCTION(BlueprintCallable, Category = "SNP|Camera")
	bool VehicleCameraBehavior(float DeltaTime, FVector& Location, FRotator& Rotation, float& FOV);
	
	bool GetCameraBehavior(const AActor* Target, const float DeltaTime, FVector& Location, FRotator& Rotation, float& FOV);

public:
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "SNP|Camera")
	TObjectPtr<ASNPVehicle> ControlledVehicle = nullptr;
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Camera")
	float OrbitRadius = 2000;
};