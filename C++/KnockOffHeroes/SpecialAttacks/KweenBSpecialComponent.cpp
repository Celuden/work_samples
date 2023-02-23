/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "KweenBSpecialComponent.hpp"

#include "AssetManager.hpp"
#include "GameObjectManager.hpp"
#include "HealthComponent.hpp"
#include "PointCounterComponent.hpp"

mmt_gd::KweenBSpecialComponent::KweenBSpecialComponent(
    GameObject& gameObject,
    GameObject& beeOne,
    GameObject& beeTwo,
    GameObject& beeThree,
    const int   attackValue) :
ISpecialAttackComponent(gameObject),
m_beeOne(beeOne),
m_beeTwo(beeTwo),
m_beeThree(beeThree),
m_specialLaunched(false),
m_specialTimer(0.f),
m_specialTimerLimit(2.f),
m_attackValue(attackValue)
{
}

bool mmt_gd::KweenBSpecialComponent::init()
{
    return true;
}

void mmt_gd::KweenBSpecialComponent::update(float deltaTime)
{
    if (m_specialLaunched)
    {
        m_specialTimer += deltaTime;

        // Set Bee Position to all players
        m_beeOne.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[0])->getPosition());
        m_beeTwo.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[1])->getPosition());
        m_beeThree.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[2])->getPosition());

        if (m_specialTimer > m_specialTimerLimit)
            stopSpecial();
    }
}

void mmt_gd::KweenBSpecialComponent::specialAttack()
{
    if (!m_specialLaunched)
    {
        m_specialLaunched = true;

        int playerCount = GameObjectManager::getInstance().getObjectCount(GameObjectTag::Player);
        int enemyCount  = GameObjectManager::getInstance().getObjectCount(GameObjectTag::Enemy);

        for (int i = 0; i < playerCount; ++i)
        {
            auto& playerId = GameObjectManager::getInstance().getGameObject("Player" + std::to_string(i))->getId();
            if (playerId != m_gameObject.getId())
                m_nonBeePlayers.push_back(playerId);
        }

        // Set Bee Position to all players
        m_beeOne.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[0])->getPosition());
        m_beeTwo.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[1])->getPosition());
        m_beeThree.setPosition(GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[2])->getPosition());

        // Player stuff
        int size = m_nonBeePlayers.size();
        for (int i = 0; i < size; ++i)
        {
            auto& player = GameObjectManager::getInstance().getGameObject(m_nonBeePlayers[i]);

            // Steal Points

            if (player->isActive())
            {
                if (!player->getComponent<HealthComponent>()->getInvincible())
                    player->getComponent<PointCounterComponent>()
                        ->stealPoints(m_gameObject.getComponent<PointCounterComponent>(), m_attackValue * 30);

                // Damage all players
                player->getComponent<HealthComponent>()->takeDamage(m_attackValue,
                                                                    *m_gameObject.getComponent<BoxCollisionComponent>());
            }
        }

        // Damage all enemies
        for (int i = 0; i < enemyCount; i++)
        {
            auto& enemy = GameObjectManager::getInstance().getGameObject("Enemy" + std::to_string(i));
            if (enemy != nullptr && enemy->isActive())
                enemy->getComponent<HealthComponent>()->takeDamage(m_attackValue,
                                                                   *m_gameObject.getComponent<BoxCollisionComponent>());
        }

        m_beeOne.setActive(true);
        m_beeTwo.setActive(true);
        m_beeThree.setActive(true);

        AssetManager::getInstance().playSound("KweenBSpecial");
    }
}

void mmt_gd::KweenBSpecialComponent::stopSpecial()
{
    m_specialLaunched = false;
    m_beeOne.setActive(false);
    m_beeTwo.setActive(false);
    m_beeThree.setActive(false);
    m_nonBeePlayers.clear();
    m_specialTimer = 0;
}
