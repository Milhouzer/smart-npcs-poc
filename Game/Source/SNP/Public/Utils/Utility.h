#pragma once
#include "CoreMinimal.h"

// Type trait to check if a type has a Get() method (for TObjectPtr)
template<typename T, typename = void>
struct THasGetMethod : std::false_type {};

template<typename T>
struct THasGetMethod<T, std::void_t<decltype(std::declval<T>().Get())>> : std::true_type {};

template<typename... Args>
bool ValidateParams(const TCHAR* FunctionName, const Args&... Arguments)
{
	bool bAllValid = true;
	int ParamIndex = 0;
        
	auto ValidateSingle = [&](const auto& Arg) {
		ParamIndex++;
            
		if constexpr (TIsPointer<typename TRemoveReference<decltype(Arg)>::Type>::Value)
		{
			if (Arg == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("%s: Parameter %d is null"), FunctionName, ParamIndex);
				bAllValid = false;
			}
		}
		else if constexpr (TIsReferenceType<decltype(Arg)>::Value)
		{
			// Handle TObjectPtr
			if constexpr (THasGetMethod<decltype(Arg)>::Value)
			{
				if (!Arg.IsValid())
				{
					UE_LOG(LogTemp, Warning, TEXT("%s: Parameter %d is invalid"), FunctionName, ParamIndex);
					bAllValid = false;
				}
			}
		}
	};
        
	(ValidateSingle(Arguments), ...);
	return bAllValid;
}

#define VALIDATE_PARAMS(...) \
	ValidateParams(ANSI_TO_TCHAR(__FUNCTION__), ##__VA_ARGS__)