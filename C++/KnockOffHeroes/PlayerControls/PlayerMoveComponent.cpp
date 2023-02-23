/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "PlayerMoveComponent.hpp"

#include "DebugDraw.hpp"
#include "GameObject.hpp"
#include "InputManager.hpp"
#include "InputManagerController.hpp"
#include "PlayerAttackComponent.hpp"
#include "RigidBodyComponent.hpp"
#include "SpecialAttackComponent.hpp"
#include "SpriteAnimationComponent.hpp"
#include "SpriteRenderComponent.hpp"
#include "VectorAlgebra2D.h"

namespace mmt_gd
{
PlayerMoveComponent::PlayerMoveComponent(
    GameObject& gameObject,
    int         playerIndex,
    const float speedValue,
    float       jumpTimer,
    bool        jumpTimerActive,
    bool        isGrounded,
    bool        isMoving) :
IComponent(gameObject),
m_playerIndex(playerIndex),
m_isMoving(isMoving),
m_isGrounded(isGrounded),
m_speedValue(speedValue),
m_jumpTimer(jumpTimer),
m_jumpTimerActive(jumpTimerActive),
m_previousDirection(1.f)
{
}

bool PlayerMoveComponent::init()
{
    return true;
}

void PlayerMoveComponent::update(const float deltaTime)
{
    const float step = 1.0f / 10.0f;

    if (m_jumpTimerActive)
        m_jumpTimer += deltaTime;

    if (m_jumpTimerActive && m_jumpTimer < step)
        m_isGrounded = false;

    else if (m_jumpTimerActive && m_jumpTimer > step)
        m_jumpTimerActive = false;

    // Get Body
    auto body = getGameObject().getComponent<RigidBodyComponent>();

    const float  velocity = 450.f + 450.f * (m_speedValue / 10);
    sf::Vector2f    jumpVec{0.f, -4600000.f};

    if (!m_isMoving && m_isGrounded)
        m_gameObject.getComponent<SpriteAnimationComponent>()->setState(PlayerAnimation::Idle);

    if (!m_gameObject.getComponent<SpecialAttackComponent>()->isSpecialInitiated())
    {
        if ((InputManagerController::getInstance().isKeyDown("right", m_playerIndex)) ||
            (InputManager::getInstance().isKeyDown("right", m_playerIndex)))
        {
            body->setPosition(body->getPosition() + sf::Vector2f{velocity, 0.0F} * deltaTime);
            runningAnimation(1.f);
        }
        if (InputManagerController::getInstance().isKeyDown("left", m_playerIndex) ||
            (InputManager::getInstance().isKeyDown("left", m_playerIndex)))
        {
            body->setPosition(body->getPosition() + sf::Vector2f{-velocity, 0.0F} * deltaTime);
            runningAnimation(-1.f);
        }
        if ((InputManagerController::getInstance().isKeyPressed("jump", m_playerIndex) && m_isGrounded) ||
            (InputManager::getInstance().isKeyPressed("up", m_playerIndex) && m_isGrounded))
        {
            body->m_impulses.push_back(jumpVec);
            m_jumpTimer       = 0.f;
            m_jumpTimerActive = true;
            m_isMoving        = true;
            m_gameObject.getComponent<SpriteAnimationComponent>()->setState(PlayerAnimation::Jumping);
            m_isGrounded = false;
        }
    }
}
void PlayerMoveComponent::runningAnimation(float direction)
{
    m_isMoving = true;
    if (m_isGrounded)
        m_gameObject.getComponent<SpriteAnimationComponent>()->setState(PlayerAnimation::Running);

    // Sprite flippage
    if (m_previousDirection != direction)
        flipSprites(direction);
}
void PlayerMoveComponent::flipSprites(float direction)
{
    m_previousDirection = direction;
    auto& sprite        = m_gameObject.getComponent<SpriteRenderComponent>()->getSprite();
    auto& attackSprite  = m_gameObject.getComponent<PlayerAttackComponent>()
                             ->getAttackGO()
                             .getComponent<SpriteRenderComponent>()
                             ->getSprite();
    sprite.setScale(direction, sprite.getScale().y);
    attackSprite.setScale(direction, attackSprite.getScale().y);

    if (direction < 0)
        sprite.setPosition(sf::Vector2f(sprite.getPosition().x + sprite.getGlobalBounds().width, sprite.getPosition().y));
    else
        sprite.setPosition(sf::Vector2f(sprite.getPosition().x - sprite.getGlobalBounds().width, sprite.getPosition().y));
}
} // namespace mmt_gd
