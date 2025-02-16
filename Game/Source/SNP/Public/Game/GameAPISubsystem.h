#pragma once

#include "CoreMinimal.h"
#include "HTTP/TalkRequest.h"
#include "GameAPISubsystem.generated.h"

class UAPIClient;

UCLASS()
class SNP_API UGameAPISubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	// Initialize and shutdown
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	// API methods
	void SendTalkCommand(const FTalkRequest& Data, TFunction<void(const FTalkResponse&, bool)> Callback);

	UFUNCTION(BlueprintCallable)
	void SendSaveCommand(const FSaveData& Data);
	
	void SendSaveCommand(const FSaveDataArray& Data);

	FLoadedData LoadData(int PlayerId, TFunction<void(const FLoadedDataArray&, bool)> Callback);
    
private:
	TObjectPtr<UAPIClient> APIClient;
};