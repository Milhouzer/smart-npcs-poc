// APIClient.h
#pragma once

#include "CoreMinimal.h"
#include "Http.h"
#include "JsonObjectConverter.h"
#include "APIClient.generated.h"

UCLASS()
class SNP_API UAPIClient : public UObject
{
	GENERATED_BODY()

public:
	UAPIClient();

	void Configure(const FString& InAddress, int32 InPort);

	template<typename TRequest>
	static FString StructToJson(const TRequest& Request)
	{
	    TSharedRef<FJsonObject> OutJsonObject = MakeShareable(new FJsonObject);
	    FJsonObjectConverter::UStructToJsonObject(TRequest::StaticStruct(), &Request, OutJsonObject, 0, 0);
	    FString OutputString;
	    TSharedRef<TJsonWriter<>> Writer = TJsonWriterFactory<>::Create(&OutputString);
	    FJsonSerializer::Serialize(OutJsonObject, Writer);
	
	    return OutputString;
	}
	
	template<typename TRequest, typename TResponse>
	void Post(const FString& Endpoint, const TRequest& Request, TFunction<void(const TResponse&, bool)> Callback)
	{
		TSharedRef<IHttpRequest> HttpRequest = CreateRequest(Endpoint, TEXT("POST"));
		FString OutputString = StructToJson(Request);
		HttpRequest->SetContentAsString(OutputString);
		UE_LOG(LogTemp, Log, TEXT("Created %s request with body: %s"), *HttpRequest->GetURL(), *OutputString);

		HttpRequest->OnProcessRequestComplete().BindLambda(
			[Callback](FHttpRequestPtr Request, FHttpResponsePtr Response, bool bWasSuccessful)
			{
				TResponse ResultStruct;
				bool bParsed = false;
            
				if (bWasSuccessful && Response.IsValid())
				{
					bParsed = FJsonObjectConverter::JsonObjectStringToUStruct(
						Response->GetContentAsString(), 
						&ResultStruct,
						0, 0);
				}
				Callback(ResultStruct, bParsed);
			});
        
		HttpRequest->ProcessRequest();
	}
	
	template<typename TResponse>
	void Get(const FString& Endpoint, TFunction<void(const TResponse&, bool)> Callback)
	{
	    TSharedRef<IHttpRequest> Request = CreateRequest(Endpoint, TEXT("GET"));
	    
	    Request->OnProcessRequestComplete().BindLambda(
	        [Callback](FHttpRequestPtr, FHttpResponsePtr Response, bool bWasSuccessful)
	        {
	            TResponse ResultStruct;
	            bool bParsed = false;
	            
	            if (bWasSuccessful && Response.IsValid())
	            {
	                bParsed = FJsonObjectConverter::JsonObjectStringToUStruct(
	                    Response->GetContentAsString(), 
	                    &ResultStruct,
	                    0, 0);
	            }
	            
	            Callback(ResultStruct, bParsed);
	        });
	        
	    Request->ProcessRequest();
	}

	template<typename TRequest, typename TResponse>
	void Put(const FString& Endpoint, const TRequest& Request, TFunction<void(const TResponse&, bool)> Callback)
	{
	    TSharedRef<IHttpRequest> HttpRequest = CreateRequest(Endpoint, TEXT("PUT"));
	    
	    FString JsonString;
	    FJsonObjectConverter::UStructToJsonObjectString(Request, JsonString);
	    HttpRequest->SetContentAsString(JsonString);

	    HttpRequest->OnProcessRequestComplete().BindLambda(
	        [Callback](FHttpRequestPtr, FHttpResponsePtr Response, bool bWasSuccessful)
	        {
	            TResponse ResultStruct;
	            bool bParsed = false;
	            
	            if (bWasSuccessful && Response.IsValid())
	            {
	                bParsed = FJsonObjectConverter::JsonObjectStringToUStruct(
	                    Response->GetContentAsString(), 
	                    &ResultStruct,
	                    0, 0);
	            }
	            
	            Callback(ResultStruct, bParsed);
	        });
	        
	    HttpRequest->ProcessRequest();
	}

	template<typename TResponse>
	void Delete(const FString& Endpoint, TFunction<void(const TResponse&, bool)> Callback)
	{
	    TSharedRef<IHttpRequest> Request = CreateRequest(Endpoint, TEXT("DELETE"));
	    
	    Request->OnProcessRequestComplete().BindLambda(
	        [Callback](FHttpRequestPtr, FHttpResponsePtr Response, bool bWasSuccessful)
	        {
	            TResponse ResultStruct;
	            bool bParsed = false;
	            
	            if (bWasSuccessful && Response.IsValid())
	            {
	                bParsed = FJsonObjectConverter::JsonObjectStringToUStruct(
	                    Response->GetContentAsString(), 
	                    &ResultStruct,
	                    0, 0);
	            }
	            
	            Callback(ResultStruct, bParsed);
	        });
	        
	    Request->ProcessRequest();
	}

private:
	TSharedRef<IHttpRequest> CreateRequest(const FString& Endpoint, const FString& Verb);
    
	UPROPERTY()
	FString BaseAddress;
    
	UPROPERTY()
	int32 Port;
};
