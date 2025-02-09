
#include "Character/SNPPlayerCameraManager.h"

#include "Character/ALSBaseCharacter.h"
#include "Character/SNPCharacter.h"
#include "Character/Animation/ALSPlayerCameraBehavior.h"
#include "Components/ALSDebugComponent.h"

#include "Kismet/KismetMathLibrary.h"


const FName NAME_CameraBehavior(TEXT("CameraBehavior"));
const FName NAME_CameraOffset_X(TEXT("CameraOffset_X"));
const FName NAME_CameraOffset_Y(TEXT("CameraOffset_Y"));
const FName NAME_CameraOffset_Z(TEXT("CameraOffset_Z"));
const FName NAME_Override_Debug(TEXT("Override_Debug"));
const FName NAME_PivotLagSpeed_X(TEXT("PivotLagSpeed_X"));
const FName NAME_PivotLagSpeed_Y(TEXT("PivotLagSpeed_Y"));
const FName NAME_PivotLagSpeed_Z(TEXT("PivotLagSpeed_Z"));
const FName NAME_PivotOffset_X(TEXT("PivotOffset_X"));
const FName NAME_PivotOffset_Y(TEXT("PivotOffset_Y"));
const FName NAME_PivotOffset_Z(TEXT("PivotOffset_Z"));
const FName NAME_RotationLagSpeed(TEXT("RotationLagSpeed"));
const FName NAME_Weight_FirstPerson(TEXT("Weight_FirstPerson"));


void ASNPPlayerCameraManager::OnPossess(ASNPVehicle* NewVehicle)
{
	check(NewVehicle)
	ControlledCharacter = nullptr;
	ControlledVehicle = NewVehicle;
	
	const FVector& TPSLoc = ControlledVehicle->GetThirdPersonPivotTarget().GetLocation();
	SetActorLocation(TPSLoc);
}

void ASNPPlayerCameraManager::UpdateViewTargetInternal(FTViewTarget& OutVT, const float DeltaTime)
{
	if (OutVT.Target)
	{
		FVector OutLocation;
		FRotator OutRotation;
		float OutFOV;

		if (OutVT.Target->IsA<ASNPVehicle>())
		{
			if (VehicleCameraBehavior(DeltaTime, OutLocation, OutRotation, OutFOV))
			{
				OutVT.POV.Location = OutLocation;
				OutVT.POV.Rotation = OutRotation;
				OutVT.POV.FOV = OutFOV;
			}
			else
			{
				OutVT.Target->CalcCamera(DeltaTime, OutVT.POV);
			}
		}
		else
		{
			Super::UpdateViewTargetInternal(OutVT, DeltaTime);
		}
	}
}

bool ASNPPlayerCameraManager::VehicleCameraBehavior(float DeltaTime, FVector& Location, FRotator& Rotation, float& FOV)
{
    if (!ControlledVehicle)
    {
        GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, TEXT("No Controlled Vehicle!"));
        return false;
    }

    // Step 1: Get the pivot location (center of the vehicle)
    const FVector Pivot = ControlledVehicle->GetActorLocation();


	const FRotator& InterpResult = FMath::RInterpTo(GetCameraRotation(),
													GetOwningPlayerController()->GetControlRotation(), DeltaTime,
													GetCameraBehaviorParam(NAME_RotationLagSpeed));

	TargetCameraRotation = GetOwningPlayerController()->GetControlRotation();
	TargetCameraLocation = Pivot - TargetCameraRotation.Vector() * OrbitRadius;
	
	Location = TargetCameraLocation;
	Rotation = TargetCameraRotation;
	
	return true;
}