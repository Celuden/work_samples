#pragma once

#include "ogl.hpp"
#include "cg_app.hpp"
#include "triangle_mesh_data.hpp"
#include "primitives.hpp"
#include "Camera.hpp"

#include <memory>
#include <chrono>
#include <array>

namespace cg
{
	class GlslProgram;
}

class App : public cg::CgApp
{

public:

	static std::shared_ptr<App> create(const unsigned w, const unsigned h);

	void render();
	void resize(const unsigned w, const unsigned h);

	void mouseButton(const int button, const int action, const int mods);
	void mousePosition(const double xpos, const double ypos);
	void keyInput(const int key, const int scancode, const int action, const int mods);
	void mouseScroll(const double xoffset, const double yoffset);

private:

	struct TriangleMesh
	{
		// these 4 member variables are necessary to represent a mesh on the GPU
		GLuint attributeBufferHandle = 0;		// attibute buffer
		GLuint indexBufferHandle = 0;			// index buffer
		GLuint vertexArrayHandle = 0;			// VAO = the "glue" between shader & buffers
		unsigned indexCount = 0;				// number of indices, needed for the glDrawElements call
	};

	struct Tree
	{
		Tree(glm::vec3 translation, glm::vec3 scale)
		{
			// Create matrix for translation and scale
			glm::mat4 translateScaleMatrix = glm::mat4(1.f);
			translateScaleMatrix[0][0] = scale.x;
			translateScaleMatrix[1][1] = scale.y;
			translateScaleMatrix[2][2] = scale.z;
			translateScaleMatrix[3][0] = translation.x;
			translateScaleMatrix[3][1] = translation.y;
			translateScaleMatrix[3][2] = translation.z;
			
			cylinderModelMatrix = translateScaleMatrix * glm::translate(glm::mat4(1.f), glm::vec3(0.f, 0.125f, 0.f))
				* glm::scale(glm::mat4(1.f), glm::vec3(0.075f, 0.25f, 0.075f));

			lowestConeMatrix = translateScaleMatrix * glm::translate(glm::mat4(1.f), glm::vec3(0.f, 0.5625f, 0.f))
				* glm::scale(glm::mat4(1.f), glm::vec3(0.3f, 0.875f, 0.3f));

			middleConeMatrix = translateScaleMatrix * glm::translate(glm::mat4(1.f), glm::vec3(0.f, 0.6375f, 0.f))
				* glm::scale(glm::mat4(1.f), glm::vec3(0.28f, 0.725f, 0.28f));

			highestConeMatrix = translateScaleMatrix * glm::translate(glm::mat4(1.f), glm::vec3(0.f, 0.7125f, 0.f))
				* glm::scale(glm::mat4(1.f), glm::vec3(0.26f, 0.575f, 0.26f));
		}

		TriangleMesh mCylinderMesh;
		TriangleMesh mLowestConeMesh;
		TriangleMesh mMiddleConeMesh;
		TriangleMesh mHighestConeMesh;

		// Matrices for geometry
		glm::mat4 cylinderModelMatrix;
		glm::mat4 lowestConeMatrix;
		glm::mat4 middleConeMatrix;
		glm::mat4 highestConeMatrix;
	};

	struct Rotorblade
	{
		Rotorblade()
		{
			zepRotorbladeMatrix = glm::mat4(1.f);
		}
		
		void setMatrix(const glm::vec3& translation, const glm::vec3& scale, const float elapsedSeconds, const float angle, const float rotationSpeed)
		{
			zepRotorbladeMatrix = glm::rotate(glm::mat4(1.f), elapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
				* glm::translate(glm::mat4(1.f), translation)
				* glm::rotate(glm::mat4(1.f), elapsedSeconds, glm::vec3(1.f, 0.f, 0.f))
				* glm::rotate(glm::mat4(1.f), glm::radians(angle), glm::vec3(1.f, 0.f, 0.f))
				* glm::scale(glm::mat4(1.f), scale);
		}
		
		TriangleMesh mZeppelinRotorbladeMesh;
		glm::mat4 zepRotorbladeMatrix;
	};

	// Helper functions for geometry are here
	void vertexSetup(std::string geometry, TriangleMesh& mesh, const int tessellation);

	// All helper functions and members for the trees are here
	std::vector<std::shared_ptr<Tree>> mTreeList;
	int rotorbladeCount;
	void treeVertexSetup(std::shared_ptr<Tree> tree);
	void renderTree(std::shared_ptr<Tree> tree, glm::mat4& projectionMatrix, glm::mat4& viewMatrix);
	void renderForest(glm::mat4& projectionMatrix, glm::mat4& viewMatrix);

	// Geometry for header is defined here
	TriangleMesh mPlaneMesh;

	// Zeppelin Geometry meshes
	TriangleMesh mZeppelinMainBodyMesh;
	TriangleMesh mZeppelinFrontSphereMesh;
	TriangleMesh mZeppelinCabinMesh;
	TriangleMesh mZeppelinCabinConnectionMesh;
	TriangleMesh mZeppelinRotorBridgeMesh;

	// Zeppelin helper functions
	std::vector<std::shared_ptr<Rotorblade>> mRotorbladeList;
	void renderZeppelin(std::shared_ptr<cg::GlslProgram> program, glm::mat4& projectionMatrix, glm::mat4& viewMatrix);
	void renderRotorblade(const glm::vec3& color, const glm::vec3& translation, const glm::vec3& scale, const float angle,
		glm::mat4& projectionMatrix, glm::mat4& viewMatrix, const float rotationSpeed);
	void zepUniformSet(std::shared_ptr<cg::GlslProgram> program, glm::vec3 lightPosition, const int pattern, const glm::vec3 kDColor, const glm::vec3 kSColor, const float shininess,
		glm::mat4& projectionMatrix, glm::mat4& viewMatrix, glm::mat4& zepMatrix, glm::mat3& zepNormalMatrix, TriangleMesh& mesh);

	// Camera
	Camera mCamera;
	float mLastX;
	float mLastY;
	bool mFirstMouse = true;

	// Lights
	glm::vec4 lightPosition;
	bool mSpotlightActive = true;
	bool mDirectionalLightActive = true;

	// Shader programs
	std::shared_ptr<cg::GlslProgram> mPhongShadingProgram = nullptr;
	std::shared_ptr<cg::GlslProgram> mTerrainProgram = nullptr;
	GLuint mHeightmapTexture = 0;
	GLuint mGrassMapTexture = 1;

	void initOglResources();
	void cleanupOglResources();

	App() = delete;
	~App() = default;
	App(const unsigned w, const unsigned h);

	struct Deleter
	{
		void operator()(App*& obj) const;
	};

	friend struct Deleter;

	// stores time point right after init was completed
	std::chrono::steady_clock::time_point mStartTime;
	// stores beginning of last frame
	std::chrono::steady_clock::time_point mLastFrameTime;
	float mElapsedSeconds = 0.f;

	// window dimensions and aspect ratio
	unsigned mWindowWidth;
	unsigned mWindowHeight;
	float mWindowAspectRatio;
};
