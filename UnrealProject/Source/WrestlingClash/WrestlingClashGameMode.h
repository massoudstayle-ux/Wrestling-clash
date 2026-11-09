#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "WrestlingClashGameMode.generated.h"

UCLASS()
class WRESTLINGCLASH_API AWrestlingClashGameMode : public AGameModeBase
{
    GENERATED_BODY()

public:
    AWrestlingClashGameMode();
    virtual void BeginPlay() override;
};