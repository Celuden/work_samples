/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "Flop4925SpecialComponent.hpp"

#include "StickToGameObjectComponent.hpp"
#include "HealthComponent.hpp"
#include "AssetManager.hpp"

mmt_gd::Flop4925SpecialComponent::Flop4925SpecialComponent(GameObject& gameObject, GameObject& barrier) :
ISpecialAttackComponent(gameObject),
m_barrier(barrier),
m_specialLaunched(false),
m_specialTimer(0.f),
m_specialTimerLimit(5.f)
{
}

bool mmt_gd::Flop4925SpecialComponent::init()
{
    return true;
}

void mmt_gd::Flop4925SpecialComponent::update(float deltaTime)
{
    if (m_specialLaunched)
    {
        m_gameObject.getComponent<HealthComponent>()->setPermaInvincible();
        m_specialTimer += deltaTime;
        if (m_specialTimer > m_specialTimerLimit)
            stopSpecial();
    }
}

void mmt_gd::Flop4925SpecialComponent::specialAttack()
{
    if (!m_specialLaunched)
    {
        AssetManager::getInstance().playSound("Flop4925Special");
        m_specialLaunched = true;
        m_barrier.setActive(true);
    }
}

void mmt_gd::Flop4925SpecialComponent::stopSpecial()
{
    m_specialLaunched = false;
    m_barrier.setActive(false);
    m_specialTimer = 0;
}
