/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once

#include "IComponent.hpp"

#include <SFML/Graphics/Rect.hpp>

namespace mmt_gd
{
class PlayerMoveComponent final : public IComponent
{
public:
    PlayerMoveComponent(GameObject& gameObject, int playerIndex, const float speedValue, float jumpTimer = 0.f, bool jumpTimerActive = false, bool isGrounded = false, bool isMoving = false);

    bool init() override;
    void update(float deltaTime) override;
    void setGrounded(bool value)
    {
        m_isGrounded = value;
    }

    bool getGrounded()
    {
        return m_isGrounded;
    }

    void setMoving(bool value)
    {
        m_isMoving = value;
    }

    bool getJumpTimerActive()
    {
        return m_jumpTimerActive;
    }

private:
    void  flipSprites(float direction);
    void  runningAnimation(float direction);
    int   m_playerIndex;
    const float m_speedValue;
    float m_jumpTimer;
    bool  m_jumpTimerActive;
    bool m_isMoving;
    bool m_isGrounded;
    float m_previousDirection;
};
} // namespace mmt_gd
