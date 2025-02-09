#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "InputActionValue.h"
#include "Vehicle/SNPVehicle.h"
#include "SNPCharacter.h"
#include "Character/ALSPlayerController.h"
#include "SNPPlayerController.generated.h"

/**
 * Player controller class for handling possession in multiplayer
 */
UCLASS(Blueprintable, BlueprintType)
class SNP_API ASNPPlayerController : public APlayerController
{
	GENERATED_BODY()

public:
	virtual void OnPossess(APawn* NewPawn) override;

	virtual void OnRep_Pawn() override;

	virtual void SetupInputComponent() override;

	virtual void BindActions(UInputMappingContext* Context);

	UFUNCTION(Server, Reliable, WithValidation)
	void Server_PossessTarget(APawn* Target);

	UFUNCTION(BlueprintImplementableEvent, meta =(DisplayName = "InteractWith"))
	void InteractWith_Implementation(AActor* Actor);
	
protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	void UpdateReticleTarget();
	
	void OnPossessCharacter(ASNPCharacter* character);
	
	void OnPossessVehicle(ASNPVehicle* vehicle);

	void SetupCharacterInputs() const;

	void SetupCharacterCamera() const;

	void SetupVehicleInputs() const;
	
	void SetupVehicleCamera() const;
	
	/* Character inputs */
	UFUNCTION()
	void ForwardMovementAction(const FInputActionValue& Value);

	UFUNCTION()
	void RightMovementAction(const FInputActionValue& Value);

	UFUNCTION()
	void CameraUpAction(const FInputActionValue& Value);

	UFUNCTION()
	void CameraRightAction(const FInputActionValue& Value);

	UFUNCTION()
	void JumpAction(const FInputActionValue& Value);

	UFUNCTION()
	void SprintAction(const FInputActionValue& Value);

	UFUNCTION()
	void AimAction(const FInputActionValue& Value);
	
	UFUNCTION()
	void InteractAction(const FInputActionValue& Value);

	UFUNCTION()
	void CameraTapAction(const FInputActionValue& Value);

	UFUNCTION()
	void CameraHeldAction(const FInputActionValue& Value);

	UFUNCTION()
	void StanceAction(const FInputActionValue& Value);

	UFUNCTION()
	void WalkAction(const FInputActionValue& Value);

	UFUNCTION()
	void RagdollAction(const FInputActionValue& Value);

	UFUNCTION()
	void VelocityDirectionAction(const FInputActionValue& Value);

	UFUNCTION()
	void LookingDirectionAction(const FInputActionValue& Value);

	// Debug character actions
	UFUNCTION()
	void DebugToggleHudAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleDebugViewAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleTracesAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleShapesAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleLayerColorsAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleCharacterInfoAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleSlomoAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugFocusedCharacterCycleAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugToggleMeshAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugOpenOverlayMenuAction(const FInputActionValue& Value);

	UFUNCTION()
	void DebugOverlayMenuCycleAction(const FInputActionValue& Value);

	
public:
	/** Main character reference */
	UPROPERTY(BlueprintReadOnly, Category = "SNP")
	TObjectPtr<ASNPCharacter> PossessedCharacter = nullptr;
	
	/** Main vehicle reference */
	UPROPERTY(BlueprintReadOnly, Category = "SNP")
	TObjectPtr<ASNPVehicle> PossessedVehicle = nullptr;
	
	/** Target actor reference */
	UPROPERTY(BlueprintReadWrite, Category = "SNP")
	AActor* ReticleTarget = nullptr;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP")
	float ReticleDistance = 2000.0;

	/** Character input mapping **/
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Input")
	TObjectPtr<UInputMappingContext> CharacterInputMappingContext = nullptr;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Input")
	TObjectPtr<UInputMappingContext> DebugInputMappingContext = nullptr;
	
	/** Vehicle input mapping **/
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Input")
	TObjectPtr<UInputMappingContext> VehicleInputMappingContext = nullptr;

	/** Camera managers **/
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Camera")
	TObjectPtr<APlayerCameraManager> CharacterCameraManager = nullptr;
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "SNP|Camera")
	TObjectPtr<APlayerCameraManager> VehicleCameraManager = nullptr;
};
