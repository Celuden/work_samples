/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "CrabooSpecialComponent.hpp"
#include "RigidBodyComponent.hpp"
#include "SpriteRenderComponent.hpp"
#include "AssetManager.hpp"

mmt_gd::CrabooSpecialComponent::CrabooSpecialComponent(GameObject& gameObject, GameObject& clawOne, GameObject& clawTwo) :
ISpecialAttackComponent(gameObject),
m_clawOne(clawOne),
m_clawTwo(clawTwo),
m_heightDisplacement(0.2f),
m_specialLaunched(false),
m_specialTimer(0.f),
m_specialTimerLimit(5.f)
{
}

bool mmt_gd::CrabooSpecialComponent::init()
{   
    // Flip claw sprite in preparation for special launch
    sf::Sprite& clawTwoSprite = m_clawTwo.getComponent<SpriteRenderComponent>()->getSprite();
    clawTwoSprite.setScale(-1.f, 1.f);
    clawTwoSprite.setPosition(clawTwoSprite.getPosition() + sf::Vector2f(clawTwoSprite.getTextureRect().width, 0));
    return true;
}

void mmt_gd::CrabooSpecialComponent::update(float deltaTime)
{
    if (m_specialLaunched)
    {
        m_specialTimer += deltaTime;
        if (m_specialTimer > m_specialTimerLimit)
            stopSpecial();
    }
}

void mmt_gd::CrabooSpecialComponent::specialAttack()
{
    if (!m_specialLaunched)
    {
        sf::FloatRect globBounds = m_gameObject.getComponent<SpriteRenderComponent>()->getSprite().getGlobalBounds();
        m_clawOne.getComponent<RigidBodyComponent>()->setPosition(
            m_gameObject.getPosition() + sf::Vector2f(globBounds.width, globBounds.height * m_heightDisplacement));
        m_clawTwo.getComponent<RigidBodyComponent>()->setPosition(
            m_gameObject.getPosition() + sf::Vector2f(0, globBounds.height * m_heightDisplacement));

        AssetManager::getInstance().playSound("CrabooSpecial");

        m_specialLaunched = true;
        m_clawOne.setActive(true);
        m_clawTwo.setActive(true);
    }
}

void mmt_gd::CrabooSpecialComponent::stopSpecial()
{
    m_specialLaunched = false;
    m_specialTimer    = 0;

    m_clawOne.setActive(false);
    m_clawTwo.setActive(false);

    m_clawOne.getComponent<RigidBodyComponent>()->setPosition(sf::Vector2f(5000, 5000));
    m_clawTwo.getComponent<RigidBodyComponent>()->setPosition(sf::Vector2f(5000, 5000));
}
