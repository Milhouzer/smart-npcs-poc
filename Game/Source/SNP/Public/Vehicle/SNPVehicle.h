// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "SNPVehicleMovementComponent.h"
#include "SNPVehicle.generated.h"

/**
 * 
 */
UCLASS()
class SNP_API ASNPVehicle : public APawn
{
	GENERATED_BODY()
	
public:
	ASNPVehicle();

	// Initialize the Vehicle's components, physics, and inputs
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	virtual UPawnMovementComponent* GetMovementComponent() const override;
	
	virtual void Accelerate(float Value);
	virtual void Turn(float Value);
	virtual void ApplyMove();

	// Input functions to control the vehicle

	UFUNCTION(BlueprintCallable, Category = "SNP|Camera System")
	virtual FTransform GetThirdPersonPivotTarget();
	
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "SNP|Input")
	void ForwardMovementAction(float Value);
	
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "SNP|Input")
	void RightMovementAction(float Value);
	
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "SNP|Input")
	void CameraUpAction(float Value);

	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "SNP|Input")
	void CameraRightAction(float Value);

protected:
	UPROPERTY()
	TObjectPtr<USNPVehicleMovementComponent> MovementComponentOverride;
	
	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float LookUpDownRate = 1.25f;

	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float LookLeftRightRate = 1.25f;
	
	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float Acceleration = 1.0f;
	
	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float MaxSpeed = 100.0f;

	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float TurnSpeed = 1.0f;
	
	UPROPERTY(EditDefaultsOnly, Category = "SNP|Input", BlueprintReadOnly)
	float TurningRadius = 1.0f;
	
	float CurrentSpeed;
	float TurnAmount;
};
