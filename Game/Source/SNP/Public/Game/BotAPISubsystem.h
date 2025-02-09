#pragma once

#include "CoreMinimal.h"
#include "HTTP/TalkRequest.h"
#include "BotAPISubsystem.generated.h"

class UAPIClient;

UCLASS()
class SNP_API UBotAPISubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	// Initialize and shutdown
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	// API methods for your commands
	void SendTalkCommand(const FTalkRequest& Data, TFunction<void(const FTalkResponse&, bool)> Callback);
    
private:
	TObjectPtr<UAPIClient> APIClient;
};