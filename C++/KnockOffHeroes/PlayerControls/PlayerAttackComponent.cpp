/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "PlayerAttackComponent.hpp"

#include "AssetManager.hpp"
#include "BoxCollisionComponent.hpp"
#include "HealthComponent.hpp"
#include "InputManager.hpp"
#include "InputManagerController.hpp"
#include "RigidBodyComponent.hpp"
#include "SpecialAttackComponent.hpp"
#include "SpriteAnimationComponent.hpp"
#include "StickToGameObjectComponent.hpp"

#include <SFML/System.hpp>
#include <iostream>


mmt_gd::PlayerAttackComponent::PlayerAttackComponent(
    GameObject& gameObject,
    GameObject& attackGO,
    const int   attackValue,
    const bool  inAttackAnimation,
    const float attackLimiter) :
IComponent(gameObject),
m_attackGO(attackGO),
m_attackValue(attackValue),
m_inAttackAnimation(inAttackAnimation),
m_attackLimiter(attackLimiter)
{
}

void mmt_gd::PlayerAttackComponent::attack()
{
    m_gameObject.getComponent<SpriteAnimationComponent>()->resetAnimation();
    m_attackGO.getComponent<SpriteAnimationComponent>()->resetAnimation();
    m_attackGO.setActive(true);
    m_inAttackAnimation   = true;
    m_timeSinceLastAttack = 0;

    AssetManager::getInstance().playSound("PlayerAttack", true);
}

bool mmt_gd::PlayerAttackComponent::init()
{
    auto attackCollisionFunction =
        [&attackValue = m_attackValue](mmt_gd::BoxCollisionComponent& slapper, const mmt_gd::BoxCollisionComponent& other)
    {
        if (other.getGameObject().getTag() == GameObjectTag::Enemy ||
            (other.getGameObject().getTag() == GameObjectTag::Player &&
             slapper.getGameObject().getComponent<StickToGameObjectComponent>()->getTarget().getId() !=
                 other.getGameObject().getId()))
        {
            if (other.getGameObject().getComponent<mmt_gd::HealthComponent>() != nullptr)
                other.getGameObject().getComponent<HealthComponent>()->takeDamage(attackValue, slapper);
        }
    };

    m_attackGO.getComponent<BoxCollisionComponent>()->registerOnCollisionFunction(attackCollisionFunction);

    return true;
}

void mmt_gd::PlayerAttackComponent::update(float deltaTime)
{
    m_timeSinceLastAttack += deltaTime;
    int playerIdx = m_gameObject.getId().back() - '0';

    // Set state if currently attacking
    if (!m_gameObject.getComponent<SpecialAttackComponent>()->isSpecialInitiated())
    {
        if (m_inAttackAnimation)
            m_gameObject.getComponent<SpriteAnimationComponent>()->setState(PlayerAnimation::Attacking);

        if (m_inAttackAnimation && m_timeSinceLastAttack > m_attackLimiter)
        {
            m_inAttackAnimation = false;
            m_attackGO.setActive(false);
        }

        if ((InputManagerController::getInstance().isKeyPressed("attack", playerIdx) &&
             m_timeSinceLastAttack > m_attackLimiter) ||
            (InputManager::getInstance().isKeyPressed("attack", playerIdx) && m_timeSinceLastAttack > m_attackLimiter))
            attack();
    }
}
