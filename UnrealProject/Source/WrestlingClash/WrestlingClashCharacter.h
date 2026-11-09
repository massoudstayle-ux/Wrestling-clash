#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "WrestlingClashCharacter.generated.h"

class UCameraComponent;
class USpringArmComponent;
class UStaticMeshComponent;

UCLASS()
class WRESTLINGCLASH_API AWrestlingClashCharacter : public ACharacter
{
    GENERATED_BODY()

public:
    AWrestlingClashCharacter();
    float GetHealth() const { return Health; }
    void SetTrainingOpponent(bool bEnabled) { bIsTrainingOpponent = bEnabled; }

    // Public entry points for future UMG touch controls and virtual joystick.
    UFUNCTION(BlueprintCallable, Category = "Mobile Input")
    void SetTouchMoveInput(float Forward, float Right);

    UFUNCTION(BlueprintCallable, Category = "Mobile Input")
    void TouchPunch();

    UFUNCTION(BlueprintCallable, Category = "Mobile Input")
    void TouchKick();

    UFUNCTION(BlueprintCallable, Category = "Mobile Input")
    void TouchLook(float YawDelta, float PitchDelta);

protected:
    virtual void Tick(float DeltaSeconds) override;
    virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;

private:
    UPROPERTY(VisibleAnywhere, Category = "Camera")
    TObjectPtr<USpringArmComponent> CameraBoom;

    UPROPERTY(VisibleAnywhere, Category = "Camera")
    TObjectPtr<UCameraComponent> FollowCamera;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> TorsoMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> HeadMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> LeftArmMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> RightArmMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> LeftLegMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> RightLegMesh;

    UPROPERTY(EditAnywhere, Category = "Combat")
    float Health = 100.0f;

    float LastAttackTime = -100.0f;

    bool bIsTrainingOpponent = false;

    void MoveForward(float Value);
    void MoveRight(float Value);
    void Punch();
    void Kick();
    void PerformAttack(float Damage, float Range);
    void ReceiveHit(float Damage);
};