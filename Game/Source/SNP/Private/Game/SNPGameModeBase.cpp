#include "Game/SNPGameModeBase.h"
#include "Character/ALSBaseCharacter.h"
#include "Game/SNPGameManager.h"

ASNPGameModeBase::ASNPGameModeBase(): GameManager(nullptr), LevelSimulationAsset(nullptr)
{
	DefaultPawnClass = AALSBaseCharacter::StaticClass();
}

void ASNPGameModeBase::BeginPlay()
{
	Super::BeginPlay();

	if (!GameManager)
	{
		GameManager = NewObject<USNPGameManager>(this);
		GameManager->Initialize(this);
	}
}

void ASNPGameModeBase::Destroyed()
{
	Super::Destroyed();

	if (GameManager)
	{
		GameManager->Deinitialize(this);
	}
}

void ASNPGameModeBase::PostLogin(APlayerController* NewPlayer)
{
	Super::PostLogin(NewPlayer);
	
	OnPlayerConnected.Broadcast(NewPlayer);
}

void ASNPGameModeBase::Logout(AController* Exiting)
{
	Super::Logout(Exiting);
	
	OnPlayerDisconnected.Broadcast(Cast<APlayerController>(Exiting));
}
