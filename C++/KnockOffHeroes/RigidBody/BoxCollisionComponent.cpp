/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "BoxCollisionComponent.hpp"
#include "DebugDraw.hpp"
#include "RenderManager.hpp"
#include "GameObjectTag.hpp"

mmt_gd::BoxCollisionComponent::BoxCollisionComponent(
    GameObject&       gameObject,
    sf::RenderWindow& renderWindow,
    sf::FloatRect     aabb,
    bool              isActive,
    bool              isTrigger,
    int               renderOrder) :
IRenderComponent(gameObject, renderWindow, renderOrder),
m_aabb(aabb),
m_isActive(isActive),
m_isTrigger(isTrigger)
{
    RenderManager::getInstance().addRenderComponent(this, renderOrder);
}

bool mmt_gd::BoxCollisionComponent::init()
{
    return true;
}

void mmt_gd::BoxCollisionComponent::update(float deltaTime)
{
    const auto& transform = m_gameObject.getTransform();
    m_transformedAabb     = transform.transformRect(m_aabb);
    
}

void mmt_gd::BoxCollisionComponent::draw()
{
    if (m_isActive)
    {
        if (m_isTrigger)
        {
            DebugDraw::getInstance().drawRectangle({m_transformedAabb.left, m_transformedAabb.top},
                                                   {m_transformedAabb.width, m_transformedAabb.height},
                                                   sf::Color::Blue,
                                                   sf::Color::Transparent);
        }
        else
        {
            DebugDraw::getInstance().drawRectangle({m_transformedAabb.left, m_transformedAabb.top},
                                                   {m_transformedAabb.width, m_transformedAabb.height},
                                                   sf::Color::Red,
                                                   sf::Color::Transparent);
        }
            
    }
}

sf::FloatRect& mmt_gd::BoxCollisionComponent::getShape()
{
    return m_aabb;
}

void mmt_gd::BoxCollisionComponent::registerOnCollisionFunction(const OnCollisionFunction& func)
{
    m_onCollisionFunctions.push_back(func);
}

void mmt_gd::BoxCollisionComponent::onCollision(BoxCollisionComponent& collider)
{
    for (const auto& f : m_onCollisionFunctions)
    {
        f(*this, collider);
    }
}
