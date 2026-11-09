#pragma once

#include "CoreMinimal.h"
#include "GameFramework/HUD.h"
#include "WrestlingClashHUD.generated.h"

UCLASS()
class WRESTLINGCLASH_API AWrestlingClashHUD : public AHUD
{
    GENERATED_BODY()

public:
    virtual void DrawHUD() override;

private:
    void DrawHealthBar(const FString& Label, float Health, float X, float Y, float Width);
};
