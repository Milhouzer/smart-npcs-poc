#pragma once

#include "CoreMinimal.h"
#include "Tickable.h"
#include "SNPGameManager.generated.h"

class ASNPGameModeBase;
class USNPGameInstance;
class APlayerController;
class FLevelsManager;

UCLASS()
class SNP_API USNPGameManager : public UObject, public FTickableGameObject
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintCallable)
	// Initialize the manager with a reference to the GameInstance
	void Initialize(ASNPGameModeBase* GameMode);

	// Cleanup bindings
	UFUNCTION(BlueprintCallable)
	void Deinitialize(ASNPGameModeBase* GameMode);

	// Begin FTickableGameObject Interface
	virtual void Tick(float DeltaTime) override;
	virtual TStatId GetStatId() const override;
	virtual bool IsTickable() const override;
	// End FTickableGameObject Interface
	
protected:
	// Called when a player connects
	UFUNCTION()
	void HandlePlayerConnected(APlayerController* PlayerController);

	// Called when a player disconnects
	UFUNCTION()
	void HandlePlayerDisconnected(APlayerController* PlayerController);

	// Separated Logic
	void HandlePlayerConnected_Server(APlayerController* PlayerController) const;
	void HandlePlayerConnected_Client(APlayerController* PlayerController) const;

	void HandlePlayerDisconnected_Server(APlayerController* PlayerController) const;
	void HandlePlayerDisconnected_Client(APlayerController* PlayerController) const;
	
private:
	// Helper
	bool IsServer() const;
	
	// Keep a weak reference to the GameInstance to avoid dangling pointers
	TWeakObjectPtr<ASNPGameModeBase> CachedGameMode;

};
