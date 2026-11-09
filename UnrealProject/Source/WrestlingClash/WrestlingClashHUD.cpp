#include "WrestlingClashHUD.h"
#include "WrestlingClashCharacter.h"
#include "Engine/Canvas.h"
#include "Engine/Engine.h"
#include "Engine/Font.h"
#include "EngineUtils.h"

void AWrestlingClashHUD::DrawHUD()
{
    Super::DrawHUD();

    if (!Canvas)
    {
        return;
    }

    AWrestlingClashCharacter* PlayerCharacter =
        Cast<AWrestlingClashCharacter>(PlayerOwner ? PlayerOwner->GetPawn() : nullptr);

    if (!PlayerCharacter)
    {
        return;
    }

    AWrestlingClashCharacter* OpponentCharacter = nullptr;
    for (TActorIterator<AWrestlingClashCharacter> It(GetWorld()); It; ++It)
    {
        if (*It != PlayerCharacter)
        {
            OpponentCharacter = *It;
            break;
        }
    }

    const float ScreenWidth = Canvas->ClipX;
    const float BarWidth = FMath::Min(300.0f, ScreenWidth * 0.34f);
    const float BarY = 36.0f;

    DrawHealthBar(TEXT("PLAYER"), PlayerCharacter->GetHealth(),
                  32.0f, BarY, BarWidth);

    if (OpponentCharacter)
    {
        DrawHealthBar(TEXT("OPPONENT"), OpponentCharacter->GetHealth(),
                      ScreenWidth - BarWidth - 32.0f, BarY, BarWidth);
    }
}

void AWrestlingClashHUD::DrawHealthBar(
    const FString& Label, float Health, float X, float Y, float Width)
{
    if (!Canvas)
    {
        return;
    }

    const float SafeHealth = FMath::Clamp(Health, 0.0f, 100.0f);
    const float BarHeight = 18.0f;
    const float FillWidth = Width * (SafeHealth / 100.0f);

    DrawText(Label, FLinearColor::White, X, Y - 22.0f, GEngine->GetSmallFont(), 1.2f);
    DrawRect(FLinearColor(0.08f, 0.08f, 0.08f, 0.90f),
             X, Y, Width, BarHeight);
    if (FillWidth > 0.0f)
    {
        const FLinearColor HealthColor = SafeHealth > 50.0f
            ? FLinearColor(0.10f, 0.80f, 0.20f, 1.0f)
            : (SafeHealth > 25.0f
                ? FLinearColor(1.0f, 0.65f, 0.05f, 1.0f)
                : FLinearColor(0.90f, 0.10f, 0.08f, 1.0f));

        DrawRect(HealthColor, X, Y, FillWidth, BarHeight);
    }

    DrawText(FString::Printf(TEXT("%.0f / 100"), SafeHealth),
             FLinearColor::White, X + 4.0f, Y + 1.0f,
             GEngine->GetSmallFont(), 0.85f);
}
