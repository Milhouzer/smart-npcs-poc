// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "SNPGameModeBase.generated.h"

class USNPGameManager;
class ULevelSimulationAsset;

UCLASS()
class SNP_API ASNPGameModeBase : public AGameModeBase
{
	GENERATED_BODY()

public:
    ASNPGameModeBase();
	virtual void BeginPlay() override;
	virtual void Destroyed() override;
	virtual void PostLogin(APlayerController* NewPlayer) override;
	virtual void Logout(AController* Exiting) override;
	

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Managers")
	USNPGameManager* GameManager;
	DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnPlayerConnected, APlayerController*, PlayerController);
	DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnPlayerDisconnected, APlayerController*, PlayerController);
	
	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnPlayerConnected OnPlayerConnected;

	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnPlayerDisconnected OnPlayerDisconnected;
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Events")
	ULevelSimulationAsset* LevelSimulationAsset;
};
