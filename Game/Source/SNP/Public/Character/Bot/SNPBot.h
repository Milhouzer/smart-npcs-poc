#pragma once

#include "Command/Interfaces/CommandRunner.h"
#include "Game/Poi/PoiComponent.h"
#include "GameFramework/Character.h"
#include "Kismet/GameplayStatics.h"
#include "SNPBot.generated.h"

class UPoiComponent;
class UInteractableComponent;

/**
 * Specialized character class, with additional features like held object etc.
 */
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnInitialized, AActor*, BotOwner);

UCLASS(Blueprintable, BlueprintType)
class SNP_API ASNPBot : public ACharacter, public ICommandRunner
{
	GENERATED_BODY()
	
public:
	ASNPBot();
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	UInteractableComponent* InteractableCmp;
	
	/** Get the actor running the command **/
	UFUNCTION(BlueprintCallable, Category = "Bot")
	void InitializeOwner(AActor* BotOwner);
	
	// Delegate to bind interaction logic
	UPROPERTY(BlueprintAssignable, Category = "Interaction")
	FOnInitialized OnInitialized;
	
	/** Get the actor running the command **/
    UFUNCTION(BlueprintCallable, Category = "Command")
	virtual AActor* GetRunner() override;

	/** Run the command */
    UFUNCTION(BlueprintCallable, Category = "Command")
	virtual void Execute(const FString& CmdName, const FCommandData& CmdData) override;
	
	// Executes a command with a given name and payload
	UFUNCTION(Server, Reliable, WithValidation, BlueprintCallable, Category = "RPC")
	void ServerExecuteCraftCommand(const FString& CmdName, const FCraftCommandData& CommandData);
	
	// Executes a command with a given name and payload
	UFUNCTION(Server, Reliable, WithValidation, BlueprintCallable, Category = "RPC")
	void ServerExecuteTalkCommand(const FString& CmdName, const FString& Message);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "POI Check")
	float MaxDetectionRadius = 1000;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "POI Check")
	TArray<TEnumAsByte<EObjectTypeQuery>> ComponentTypesToCheck;
private:
	
	TArray<UPoiComponent*> CheckForNearbyPOIs() const
	{
		TArray<UPoiComponent*> PoiComponents;
		TArray<AActor*> OutActors;
		
		const TArray<TEnumAsByte<EObjectTypeQuery>> ObjectTypes = 
		{
			UEngineTypes::ConvertToObjectType(ECC_WorldStatic),
			UEngineTypes::ConvertToObjectType(ECC_WorldDynamic)
		};
		
		if(!UKismetSystemLibrary::SphereOverlapActors(
			GetWorld(),
			GetActorLocation(),
			MaxDetectionRadius,
			ObjectTypes,
			AActor::StaticClass(),
			TArray<AActor*>(),
			OutActors
		))
		{
			UE_LOG(LogTemp, Error, TEXT("No actors in sphere Failed"));
			return PoiComponents;
		}

		Algo::TransformIf(
			OutActors, 
			PoiComponents,
			[](const AActor* Actor) { return Actor->FindComponentByClass<UPoiComponent>() != nullptr; },
			[](const AActor* Actor) { return Actor->FindComponentByClass<UPoiComponent>(); }
		);

		return PoiComponents;
	}
};
