/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "GameObject.hpp"
#include "IRenderComponent.hpp"

namespace mmt_gd
{
class BoxCollisionComponent final : public IRenderComponent
{
public:
    using OnCollisionFunction = std::function<void(BoxCollisionComponent&, BoxCollisionComponent&)>;

    BoxCollisionComponent(GameObject&       gameObject,
                          sf::RenderWindow& renderWindow,
                          sf::FloatRect     aabb,
                          bool              isActive    = true,
                          bool              isTrigger   = false,
                          int               renderOrder = 14);

    bool init() override;
    void update(float deltaTime) override;
    void draw() override;
    void onCollision(BoxCollisionComponent& collider);
    void registerOnCollisionFunction(const OnCollisionFunction& func);

    bool           m_isTrigger;
    bool           m_isActive;

    sf::FloatRect& getShape();

    bool isActive() const
    {
        return m_isActive;
    }

    void setActive(const bool isActive)
    {
        m_isActive = isActive;
    }

private:
    sf::FloatRect      m_aabb;
    std::list<OnCollisionFunction> m_onCollisionFunctions;
    sf::FloatRect                  m_transformedAabb;
};
} // namespace mmt_gd
