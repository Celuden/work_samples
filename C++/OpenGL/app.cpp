#include "app.hpp"
#include "primitives.hpp"
#include "triangle_mesh_data.hpp"
#include "glsl.hpp"
#include "Camera.hpp"

#include <iostream>
#include <GLFW/glfw3.h>

#define STB_IMAGE_IMPLEMENTATION
#include <STBI\stb_image.h>

void App::Deleter::operator()(App*& obj) const
{
	if (obj == nullptr)
		return;

	obj->cleanupOglResources();

	delete obj;
	obj = nullptr;
}

std::shared_ptr< App > App::create(const unsigned w, const unsigned h)
{
	std::shared_ptr<App> result(new App(w, h), App::Deleter());
	result->initOglResources();
	result->mStartTime = std::chrono::high_resolution_clock::now();
	result->mLastFrameTime = result->mStartTime;
	return result;
}

App::App(const unsigned w, const unsigned h) : CgApp(),
	mWindowWidth(w),
	mWindowHeight(h),
	mWindowAspectRatio(w / static_cast<float>(h)),
	mCamera(Camera(glm::vec3(0.f, 2.f, 5.f), glm::vec3(0.f, 1.f, 0.f), glm::vec3(0.f, 0.f, -1.f), -90.f, 0.f))
{
}

void App::resize(const unsigned w, const unsigned h)
{
	// window size and aspect ratio are needed for viewport & projection matrix
	mWindowWidth = w;
	mWindowHeight = h;
	mWindowAspectRatio = mWindowWidth / static_cast<float>(h);
}

void App::treeVertexSetup(std::shared_ptr<Tree> tree)
{
	vertexSetup("cylinder", tree->mCylinderMesh, 30);
	vertexSetup("cone", tree->mLowestConeMesh, 30);
	vertexSetup("cone", tree->mMiddleConeMesh, 30);
	vertexSetup("cone", tree->mHighestConeMesh, 30);
	mTreeList.push_back(tree);
}

void App::vertexSetup(std::string geometry, TriangleMesh& mesh, const int tessellation)
{
	cg::TriangleMeshData tmpData;

	if (geometry == "plane")
		cg::SimplePrimitives::generatePlane(tessellation, tessellation, tmpData);
	else if (geometry == "cylinder")
		cg::SimplePrimitives::generateCylinder(tessellation, tessellation, tmpData);
	else if (geometry == "cone")
		cg::SimplePrimitives::generateCone(tessellation, tessellation, tmpData);
	else if (geometry == "sphere")
		cg::SimplePrimitives::generateSphere(tessellation, tessellation, tmpData);
	else if (geometry == "cube")
		cg::SimplePrimitives::generateCube(tmpData);
	else
		std::cout << "WEE OO WEE OO, WRONG GEOMETRY, TYPO?" << std::endl;

	mesh.indexCount = (unsigned)tmpData.indices.size();

	// create a buffer object for storing vertex attributes
	glGenBuffers(1, &mesh.attributeBufferHandle);
	glBindBuffer(GL_ARRAY_BUFFER, mesh.attributeBufferHandle);
	glBufferData(GL_ARRAY_BUFFER, sizeof(cg::BaseVertex) * tmpData.attributes.size(),
		tmpData.attributes.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, 0);

	// create another buffer object for storing indices
	glGenBuffers(1, &mesh.indexBufferHandle);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh.indexBufferHandle);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * tmpData.indices.size(),
		tmpData.indices.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

	// finally, create vertex array object
	glGenVertexArrays(1, &mesh.vertexArrayHandle);
	glBindVertexArray(mesh.vertexArrayHandle);
	// vertex array object stores the index buffer binding, i.e. which buffer object the indices come from
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh.indexBufferHandle);
	// vertex array object stores the definitions for each enabled vertex attribute
	// including
	// a) which vertex buffer object is the source for the attribute heightmapData (here, we only use a single vertex buffer object)
	glBindBuffer(GL_ARRAY_BUFFER, mesh.attributeBufferHandle);
	// b) which attributes are enabled (here, it's only 0)
	// c) attribute specific details: location = 0, 1 attribute consists of 3 floats, byte stride and offset within the buffer
	// location 0 = vertexPosition
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(cg::BaseVertex), (void*)(offsetof(cg::BaseVertex, position)));
	glEnableVertexAttribArray(0);
	// location 1 = normals
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(cg::BaseVertex), (void*)(offsetof(cg::BaseVertex, normal)));
	glEnableVertexAttribArray(1);
	// location 2 = vertex texture coordinates
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(cg::BaseVertex), (void*)(offsetof(cg::BaseVertex, texcoord)));
	glEnableVertexAttribArray(2);
	glBindVertexArray(0);
}

void App::renderTree(std::shared_ptr<Tree> tree, glm::mat4& projectionMatrix, glm::mat4& viewMatrix)
{
	// Normal matrices
	glm::mat3 cylinderNormalModelMatrix = glm::inverse(glm::transpose(tree->cylinderModelMatrix));
	glm::mat3 lowestConeNormalMatrix = glm::inverse(glm::transpose(tree->lowestConeMatrix));
	glm::mat3 middleConeNormalMatrix = glm::inverse(glm::transpose(tree->middleConeMatrix));
	glm::mat3 highestConeNormalMatrix = glm::inverse(glm::transpose(tree->highestConeMatrix));

	// Draw tree here, order: trunk --> lowestCone --> middleCone --> highestCone
	mPhongShadingProgram->setUniformIVal("pattern", 0);
	mPhongShadingProgram->setUniformVec3("material.kD", glm::vec3(0.4f, 0.2f, 0.f));
	mPhongShadingProgram->setUniformVec3("material.kS", glm::vec3(0.4f, 0.2f, 0.f));
	mPhongShadingProgram->setUniformVal("material.shininess", 2.f);
	mPhongShadingProgram->setUniformMat4("object2clip", projectionMatrix * viewMatrix * tree->cylinderModelMatrix);
	mPhongShadingProgram->setUniformMat4("object2world", tree->cylinderModelMatrix);
	mPhongShadingProgram->setUniformMat3("normal2world", cylinderNormalModelMatrix);
	glBindVertexArray(tree->mCylinderMesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, tree->mCylinderMesh.indexCount, GL_UNSIGNED_INT, nullptr);

	mPhongShadingProgram->setUniformIVal("pattern", 1);
	mPhongShadingProgram->setUniformVec3("material.kD", glm::vec3(0.08f, 0.28f, 0.2f));
	mPhongShadingProgram->setUniformVec3("material.kS", glm::vec3(0.08f, 0.28f, 0.2f));
	mPhongShadingProgram->setUniformVal("material.shininess", 2.f);
	mPhongShadingProgram->setUniformMat4("object2clip", projectionMatrix * viewMatrix * tree->lowestConeMatrix);
	mPhongShadingProgram->setUniformMat4("object2world", tree->lowestConeMatrix);
	mPhongShadingProgram->setUniformMat3("normal2world", lowestConeNormalMatrix);
	glBindVertexArray(tree->mLowestConeMesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, tree->mLowestConeMesh.indexCount, GL_UNSIGNED_INT, nullptr);

	mPhongShadingProgram->setUniformVec3("material.kD", glm::vec3(0.16f, 0.66f, 0.4f));
	mPhongShadingProgram->setUniformVec3("material.kS", glm::vec3(0.16f, 0.66f, 0.4f));
	mPhongShadingProgram->setUniformVal("material.shininess", 2.f);
	mPhongShadingProgram->setUniformMat4("object2clip", projectionMatrix * viewMatrix * tree->middleConeMatrix);
	mPhongShadingProgram->setUniformMat4("object2world", tree->middleConeMatrix);
	mPhongShadingProgram->setUniformMat3("normal2world", middleConeNormalMatrix);
	glBindVertexArray(tree->mMiddleConeMesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, tree->mMiddleConeMesh.indexCount, GL_UNSIGNED_INT, nullptr);

	mPhongShadingProgram->setUniformVec3("material.kD", glm::vec3(0.08f, 0.28f, 0.2f));
	mPhongShadingProgram->setUniformVec3("material.kS", glm::vec3(0.08f, 0.28f, 0.2f));
	mPhongShadingProgram->setUniformVal("material.shininess", 2.f);
	mPhongShadingProgram->setUniformMat4("object2clip", projectionMatrix * viewMatrix * tree->highestConeMatrix);
	mPhongShadingProgram->setUniformMat4("object2world", tree->highestConeMatrix);
	mPhongShadingProgram->setUniformMat3("normal2world", highestConeNormalMatrix);
	glBindVertexArray(tree->mHighestConeMesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, tree->mHighestConeMesh.indexCount, GL_UNSIGNED_INT, nullptr);
}

void App::renderForest(glm::mat4& projectionMatrix, glm::mat4& viewMatrix)
{
	for (const auto& t : mTreeList)
		renderTree(t, projectionMatrix, viewMatrix);
}

void App::initOglResources()
{
	mPhongShadingProgram = cg::GlslProgram::create(
		{
			{ cg::ShaderType::VERTEX_SHADER, cg::loadShaderSource("..//shaders//phong_shading.vert") },
			{ cg::ShaderType::FRAGMENT_SHADER, cg::loadShaderSource("..//shaders//phong_shading.frag") },
		}
	);
	mTerrainProgram = cg::GlslProgram::create(
		{
			{ cg::ShaderType::VERTEX_SHADER, cg::loadShaderSource("..//shaders//terrain.vert") },
			{ cg::ShaderType::FRAGMENT_SHADER, cg::loadShaderSource("..//shaders//terrain.frag") },
		}
	);

	// Setup vertex stuff for geometry
	vertexSetup("plane", mPlaneMesh, 100);
	vertexSetup("sphere", mZeppelinMainBodyMesh, 30);
	vertexSetup("sphere", mZeppelinFrontSphereMesh, 30);
	vertexSetup("cube", mZeppelinCabinMesh, 30);
	vertexSetup("cube", mZeppelinCabinConnectionMesh, 30);
	vertexSetup("cube", mZeppelinRotorBridgeMesh, 30);

	// 20 Trees with corresponding offset
	std::shared_ptr<Tree> treeOne = std::make_shared<Tree>( glm::vec3(0.f, 0.5f, -2.f) , glm::vec3(1.f) );
	treeVertexSetup(treeOne);
	std::shared_ptr<Tree> treeTwo = std::make_shared<Tree>( glm::vec3(-0.4f, 0.32f, 1.6f) , glm::vec3(1.3f) );
	treeVertexSetup(treeTwo);
	std::shared_ptr<Tree> treeThree = std::make_shared<Tree>( glm::vec3(1.5f, 0.45f, 2.5f) , glm::vec3(1.6f) );
	treeVertexSetup(treeThree);
	std::shared_ptr<Tree> treeFour = std::make_shared<Tree>( glm::vec3(-2.3f, 0.38f, -1.4f) , glm::vec3(0.7f) );
	treeVertexSetup(treeFour);
	std::shared_ptr<Tree> treeFive = std::make_shared<Tree>( glm::vec3(-0.17f, 0.43f, 2.5f) , glm::vec3(0.5f) );
	treeVertexSetup(treeFive);
	std::shared_ptr<Tree> treeSix = std::make_shared<Tree>( glm::vec3(0.8f, 0.45f, 3.f) , glm::vec3(0.6f) );
	treeVertexSetup(treeSix);
	std::shared_ptr<Tree> treeSeven = std::make_shared<Tree>( glm::vec3(3.8f, 0.35f, 4.3f) , glm::vec3(2.6f) );
	treeVertexSetup(treeSeven);
	std::shared_ptr<Tree> treeEight = std::make_shared<Tree>( glm::vec3(2.3f, 0.35f, -3.6f) , glm::vec3(1.6f) );
	treeVertexSetup(treeEight);
	std::shared_ptr<Tree> treeNine = std::make_shared<Tree>( glm::vec3(-0.8f, 0.55f, -4.f) , glm::vec3(1.f) );
	treeVertexSetup(treeNine);
	std::shared_ptr<Tree> treeTen = std::make_shared<Tree>( glm::vec3(-1.f, 0.42f, -3.5f) , glm::vec3(1.2f) );
	treeVertexSetup(treeTen);
	std::shared_ptr<Tree> treeEleven = std::make_shared<Tree>( glm::vec3(-1.5f, 0.40f, 0.2f) , glm::vec3(1.7f) );
	treeVertexSetup(treeEleven);
	std::shared_ptr<Tree> treeTwelve = std::make_shared<Tree>( glm::vec3(2.1f, 0.45f, -0.7f) , glm::vec3(1.8f) );
	treeVertexSetup(treeTwelve);
	std::shared_ptr<Tree> treeThirteen = std::make_shared<Tree>( glm::vec3(2.3f, 0.35f, 1.5f) , glm::vec3(1.1f) );
	treeVertexSetup(treeThirteen);
	std::shared_ptr<Tree> treeFourteen = std::make_shared<Tree>( glm::vec3(-2.4f, 0.48f, 4.3f) , glm::vec3(0.9f) );
	treeVertexSetup(treeFourteen);
	std::shared_ptr<Tree> treeFifteen = std::make_shared<Tree>( glm::vec3(-3.6f, 0.4f, -4.4f) , glm::vec3(1.4f) );
	treeVertexSetup(treeFifteen);
	std::shared_ptr<Tree> treeSixteen = std::make_shared<Tree>( glm::vec3(-1.4f, 0.4f, 2.2f) , glm::vec3(0.8f) );
	treeVertexSetup(treeSixteen);
	std::shared_ptr<Tree> treeSeventeen = std::make_shared<Tree>( glm::vec3(-0.15f, 0.5f, -3.8f) , glm::vec3(1.3f) );
	treeVertexSetup(treeSeventeen);
	std::shared_ptr<Tree> treeEighteen = std::make_shared<Tree>(glm::vec3(-1.1f, 0.41f, 1.8f), glm::vec3(0.6f));
	treeVertexSetup(treeEighteen);
	std::shared_ptr<Tree> treeNineteen = std::make_shared<Tree>( glm::vec3(-1.1f, 0.35f, -0.8f) , glm::vec3(0.8f) );
	treeVertexSetup(treeNineteen);
	std::shared_ptr<Tree> treeTwenty = std::make_shared<Tree>( glm::vec3(1.2f, 0.35f, 0.95f) , glm::vec3(0.7f) );
	treeVertexSetup(treeTwenty);

	// 18 Rotorblades
	for (int i = 0; i < 18; ++i)
	{
		std::shared_ptr<Rotorblade> rotorblade = std::make_shared<Rotorblade>();
		vertexSetup("cube", rotorblade->mZeppelinRotorbladeMesh, 30);
		mRotorbladeList.push_back(rotorblade);
	}

	// Load Height Map Texture
	int width, height, nChannels;
	unsigned char* heightmapData = stbi_load("..//terrain//heightmap.png", &width, &height, &nChannels, 0);

	glm::vec4 black(0.f);

	glGenTextures(1, &mHeightmapTexture);
	glBindTexture(GL_TEXTURE_2D, mHeightmapTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, heightmapData);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, &black[0]);
	glBindTexture(GL_TEXTURE_2D, 0);

	STBI_FREE(heightmapData);
	heightmapData = nullptr;

	// Load Grass Texture
	width, height, nChannels;
	unsigned char* grassData = stbi_load("..//terrain//Grass.png", &width, &height, &nChannels, 0);

	glGenTextures(1, &mGrassMapTexture);
	glBindTexture(GL_TEXTURE_2D, mGrassMapTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, grassData);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, &black[0]);
	glBindTexture(GL_TEXTURE_2D, 0);

	STBI_FREE(grassData);
	grassData = nullptr;
}

void App::cleanupOglResources()
{
	mPhongShadingProgram = nullptr;

	glDeleteVertexArrays(1, &mPlaneMesh.vertexArrayHandle);
	glDeleteBuffers(1, &mPlaneMesh.indexBufferHandle);
	glDeleteBuffers(1, &mPlaneMesh.attributeBufferHandle);
}

void App::render()
{
	std::chrono::steady_clock::time_point now = std::chrono::steady_clock::now();
	float nanoseconds = std::chrono::duration<float, std::nano>(now - mLastFrameTime).count();
	mElapsedSeconds += 1e-9f * nanoseconds;
	mLastFrameTime = now;

	// Update camera
	float deltaTime = 1e-9f * nanoseconds;
	mCamera.updateCamera(deltaTime);

	// view & projection matrices
	glm::mat4 projectionMatrix = glm::perspective(glm::radians(mCamera.m_fov), mWindowAspectRatio, 0.1f, 100.f);
	glm::mat4 viewMatrix = mCamera.getLookAtMatrix();
	
	// Plane matrix
	glm::mat4 planeModelMatrix = glm::mat4_cast(glm::fquat(glm::vec3(0.f, 0.f, 1.f), glm::vec3(0.f, 1.f, 0.f)))
		* glm::scale(glm::mat4(1.f), glm::vec3(5.f, 5.f, 1.f));
	glm::mat3 planeNormalMatrix = glm::inverse(glm::transpose(planeModelMatrix));

	// use this as ambient light color once you implement shading
	glm::vec3 ambientLight = glm::vec3(0.33f, 0.66f, 0.99f);

	// clear window to ambient color -> you no longer have a pitch black background
	glClearColor(ambientLight.x, ambientLight.y, ambientLight.z, 1.f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glViewport(0, 0, mWindowWidth, mWindowHeight);

	glEnable(GL_DEPTH_TEST);
	glEnable(GL_MULTISAMPLE);
	glEnable(GL_CULL_FACE);

	// this enables "wireframe mode", where triangles are rasterized as outlines
	//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
	
	// Plane - Terrain Shader
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, mHeightmapTexture);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, mGrassMapTexture);

	cg::GlslProgram::useProgram(mTerrainProgram);
	mTerrainProgram->setUniformIVal("pattern", 0);
	mTerrainProgram->setUniformTexVal("heightmap", 0);
	mTerrainProgram->setUniformVal("heightmapScaling", 0.6f); // Heightmap scaling
	mTerrainProgram->setUniformTexVal("grassTexture", 1);
	mTerrainProgram->setUniformVal("grassRepeat", 5.f); // How often does the grass repeat?
	mTerrainProgram->setUniformVec3("cameraPosition", mCamera.m_position);

	mTerrainProgram->setUniformBVal("spotLightIsActive", mSpotlightActive);
	if (mSpotlightActive)
		mTerrainProgram->setUniformVec3("spotLight.color", glm::vec3(0.3f));
	else
		mTerrainProgram->setUniformVec3("spotLight.color", glm::vec3(0.f));

	mTerrainProgram->setUniformVec3("spotLight.position", glm::vec3(lightPosition));
	mTerrainProgram->setUniformVec3("spotLight.spotDirection", glm::vec3(0.f, -1.f, 0.f));
	const float cutOffAngle = glm::cos(glm::radians(90.f / 2));
	mTerrainProgram->setUniformVal("spotLight.cutOffAngle", cutOffAngle);

	if (mDirectionalLightActive)
		mTerrainProgram->setUniformVec3("ambientLightColor", ambientLight); // Ambient light color here
	else
		mTerrainProgram->setUniformVec3("ambientLightColor", glm::vec3(0.f, 0.f, 0.f));
	mTerrainProgram->setUniformVec3("material.kD", glm::vec3(0.44f, 1.f, 0.22f));
	mTerrainProgram->setUniformVec3("material.kS", 0.5f * glm::vec3(0.44f, 1.f, 0.22f));
	mTerrainProgram->setUniformVal("material.shininess", 2.f);
	mTerrainProgram->setUniformMat4("object2clip", projectionMatrix * viewMatrix * planeModelMatrix);
	mTerrainProgram->setUniformMat4("object2world", planeModelMatrix);
	mTerrainProgram->setUniformMat3("normal2world", planeNormalMatrix);
	glBindVertexArray(mPlaneMesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, mPlaneMesh.indexCount, GL_UNSIGNED_INT, nullptr);

	// Phong Shading from here on out
	cg::GlslProgram::useProgram(mPhongShadingProgram);
	mPhongShadingProgram->setUniformIVal("pattern", 0);
	mPhongShadingProgram->setUniformVec3("cameraPosition", mCamera.m_position);

	mPhongShadingProgram->setUniformBVal("spotLightIsActive", mSpotlightActive);
	if (mSpotlightActive)
		mPhongShadingProgram->setUniformVec3("spotLight.color", glm::vec3(0.3f));
	else
		mPhongShadingProgram->setUniformVec3("spotLight.color", glm::vec3(0.f));

	mPhongShadingProgram->setUniformVec3("spotLight.spotDirection", glm::vec3(0.f, -1.f, 0.f));
	mPhongShadingProgram->setUniformVal("spotLight.cutOffAngle", cutOffAngle);

	if (mDirectionalLightActive)
		mPhongShadingProgram->setUniformVec3("ambientLightColor", ambientLight);
	else
		mPhongShadingProgram->setUniformVec3("ambientLightColor", glm::vec3(0.f, 0.f, 0.f));

	// Draw forest - 20 trees
	renderForest(projectionMatrix, viewMatrix);
	mPhongShadingProgram->setUniformIVal("pattern", 0);

	// Draw zeppelin
	renderZeppelin(mPhongShadingProgram, projectionMatrix, viewMatrix);

	// this disables "wireframe mode", and renders triangles as filled (=default mode)
	glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

	glBindVertexArray(0);
	cg::GlslProgram::useProgram(nullptr);
}

void App::renderZeppelin(std::shared_ptr<cg::GlslProgram> program, glm::mat4& projectionMatrix, glm::mat4& viewMatrix)
{
	constexpr float rotationSpeed = 0.5f;
	static const glm::vec3 blue = { 0.26f, 0.42f, 0.88f };
	static const glm::vec3 black = { 0.f, 0.f, 0.f };
	static const glm::vec3 orange = { 1.f, 0.65f, 0.f };
	static const glm::vec3 gray = { 0.5f, 0.5f, 0.5f };
	static const glm::vec3 white = { 1.f, 1.f, 1.f };

	// Zeppelin matrices
	glm::mat4 zepMainBodyMatrix = glm::rotate(glm::mat4(1.f), mElapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
		* glm::translate(glm::mat4(1.f), glm::vec3(0.f, 4.f, -3.f))
		* glm::scale(glm::mat4(1.f), glm::vec3(2.2f, 1.f, 1.f));

	glm::mat4 zepFrontSphereMatrix = glm::rotate(glm::mat4(1.f), mElapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
		* glm::translate(glm::mat4(1.f), glm::vec3(-2.05f, 4.f, -3.f))
		* glm::scale(glm::mat4(1.f), glm::vec3(0.33f));

	glm::mat4 zepCabinMatrix = glm::rotate(glm::mat4(1.f), mElapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
		* glm::translate(glm::mat4(1.f), glm::vec3(0.f, 2.9f, -3.f))
		* glm::scale(glm::mat4(1.f), glm::vec3(0.4f, 0.225f, 0.25f));

	glm::mat4 zepCabinConnectionMatrix = glm::rotate(glm::mat4(1.f), mElapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
		* glm::translate(glm::mat4(1.f), glm::vec3(0.f, 3.2f, -3.f))
		* glm::scale(glm::mat4(1.f), glm::vec3(0.5f, 0.275f, 0.3f));

	glm::mat4 zepRotorBridgeMatrix = glm::rotate(glm::mat4(1.f), mElapsedSeconds * rotationSpeed, glm::vec3(0.f, 1.f, 0.f))
		* glm::translate(glm::mat4(1.f), glm::vec3(0.5f, 3.6f, -3.f))
		* glm::scale(glm::mat4(1.f), glm::vec3(0.1f, 0.01f, 1.5f));

	// Normal matrices
	glm::mat3 zepMainBodyNormalMatrix = glm::inverse(glm::transpose(zepMainBodyMatrix));
	glm::mat3 zepFrontSphereNormalMatrix = glm::inverse(glm::transpose(zepFrontSphereMatrix));
	glm::mat3 zepCabinNormalMatrix = glm::inverse(glm::transpose(zepCabinMatrix));
	glm::mat3 zepCabinConnectionNormalMatrix = glm::inverse(glm::transpose(zepCabinConnectionMatrix));
	glm::mat3 zepRotorBridgeNormalMatrix = glm::inverse(glm::transpose(zepRotorBridgeMatrix));

	// Light
	lightPosition = zepCabinMatrix[3];

	// Main Body (Sphere)
	zepUniformSet(program, lightPosition, 2, orange, white, 50.f, projectionMatrix, viewMatrix, zepMainBodyMatrix, zepMainBodyNormalMatrix, mZeppelinMainBodyMesh);

	// Sphere in front
	zepUniformSet(program, lightPosition, 1, blue, white, 50.f, projectionMatrix, viewMatrix, zepFrontSphereMatrix, zepFrontSphereNormalMatrix, mZeppelinFrontSphereMesh);

	// Cabin
	zepUniformSet(program, lightPosition, 0, gray, white, 50.f, projectionMatrix, viewMatrix, zepCabinMatrix, zepCabinNormalMatrix, mZeppelinCabinMesh);

	// Cabin connection to zeppelin
	zepUniformSet(program, lightPosition, 0, black, white, 50.f, projectionMatrix, viewMatrix, zepCabinConnectionMatrix, zepCabinConnectionNormalMatrix, mZeppelinCabinConnectionMesh);

	// The "bridge" between the small rotorblades
	zepUniformSet(program, lightPosition, 0, blue, white, 50.f, projectionMatrix, viewMatrix, zepRotorBridgeMatrix, zepRotorBridgeNormalMatrix, mZeppelinRotorBridgeMesh);

	// Render rotor blades
	rotorbladeCount = 0;
	for (int i = 0; i < 180; i += 30)
	{
		// Big one
		renderRotorblade(black, glm::vec3{ 2.2f, 4.f, -3.f }, glm::vec3{ 0.015f, 0.7f, 0.03f }, static_cast<float>(i), projectionMatrix, viewMatrix, rotationSpeed);

		// Small front rotor blades
		renderRotorblade(black, glm::vec3{ 0.4f, 3.6f, -1.5f }, glm::vec3{ 0.005f, 0.23f, 0.01f }, static_cast<float>(i), projectionMatrix, viewMatrix, rotationSpeed);

		// Small back rotor blades
		renderRotorblade(black, glm::vec3{ 0.4f, 3.6f, -4.5f }, glm::vec3{ 0.005f, 0.23f, 0.01f }, static_cast<float>(i), projectionMatrix, viewMatrix, rotationSpeed);
	}
}

void App::renderRotorblade(const glm::vec3& color, const glm::vec3& translation, const glm::vec3& scale, const float angle, glm::mat4& projectionMatrix, glm::mat4& viewMatrix, const float rotationSpeed)
{
	// Set the mesh with correct values
	mRotorbladeList[rotorbladeCount]->setMatrix(translation, scale, mElapsedSeconds, angle, rotationSpeed);

	// Normal matrix
	glm::mat3 zepRotorbladeNormalMatrix = glm::inverse(glm::transpose(mRotorbladeList[rotorbladeCount]->zepRotorbladeMatrix));
	
	zepUniformSet(mPhongShadingProgram, lightPosition, 0, color, glm::vec3(1.f, 1.f, 1.f), 50.f, projectionMatrix, viewMatrix, 
		mRotorbladeList[rotorbladeCount]->zepRotorbladeMatrix, zepRotorbladeNormalMatrix, mRotorbladeList[rotorbladeCount]->mZeppelinRotorbladeMesh);

	rotorbladeCount++;
}

void App::zepUniformSet(std::shared_ptr<cg::GlslProgram> program, glm::vec3 lightPosition, const int pattern, const glm::vec3 kDColor, const glm::vec3 kSColor, const float shininess,
	glm::mat4& projectionMatrix, glm::mat4& viewMatrix, glm::mat4& zepMatrix, glm::mat3& zepNormalMatrix, TriangleMesh& mesh)
{
	program->setUniformVec3("spotLight.position", glm::vec3(lightPosition));
	program->setUniformIVal("pattern", pattern);
	program->setUniformVec3("material.kD", kDColor);
	program->setUniformVec3("material.kS", kSColor);
	program->setUniformVal("material.shininess", shininess);
	program->setUniformMat4("object2clip", projectionMatrix * viewMatrix * zepMatrix);
	program->setUniformMat4("object2world", zepMatrix);
	program->setUniformMat3("normal2world", zepNormalMatrix);
	glBindVertexArray(mesh.vertexArrayHandle);
	glDrawElements(GL_TRIANGLES, mesh.indexCount, GL_UNSIGNED_INT, nullptr);
}

void App::mouseButton(const int button, const int action, const int mods)
{
	if (button == GLFW_MOUSE_BUTTON_LEFT)
	{
		if (action == GLFW_PRESS)
			mCamera.m_rotateCamera = true;
		else if (action == GLFW_RELEASE)
			mCamera.m_rotateCamera = false;
	}
}

void App::mousePosition(const double xpos, const double ypos)
{
	if (mFirstMouse)
	{
		mLastX = static_cast<float>(xpos);
		mLastY = static_cast<float>(ypos);
		mFirstMouse = false;
	}

	static const float mouseSensitivity = 0.1f;

	float x = (static_cast<float>(xpos) - mLastX) * mouseSensitivity;
	float y = (static_cast<float>(ypos) - mLastY) * mouseSensitivity;
	mLastX = static_cast<float>(xpos);
	mLastY = static_cast<float>(ypos);

	if (mCamera.m_rotateCamera)
	{
		mCamera.m_yaw += x;
		mCamera.m_pitch -= y;

		if (mCamera.m_pitch > 89.f)
			mCamera.m_pitch = 89.f;
		if (mCamera.m_pitch < -89.f)
			mCamera.m_pitch = -89.f;
	}
}

void App::keyInput(const int key, const int scancode, const int action, const int mods)
{
	switch (key)
	{
		case GLFW_KEY_W:
			action == GLFW_RELEASE ? mCamera.setMovement(CameraMovement::NONE) : mCamera.setMovement(CameraMovement::FORWARD);
			break;
		case GLFW_KEY_A:
			action == GLFW_RELEASE ? mCamera.setMovement(CameraMovement::NONE) : mCamera.setMovement(CameraMovement::LEFT);
			break;
		case GLFW_KEY_S:
			action == GLFW_RELEASE ? mCamera.setMovement(CameraMovement::NONE) : mCamera.setMovement(CameraMovement::BACKWARD);
			break;
		case GLFW_KEY_D:
			action == GLFW_RELEASE ? mCamera.setMovement(CameraMovement::NONE) : mCamera.setMovement(CameraMovement::RIGHT);
			break;
		case GLFW_KEY_F1:
			if (action == GLFW_PRESS)
				mSpotlightActive = !mSpotlightActive;
			break;
		case GLFW_KEY_F2:
			if (action == GLFW_PRESS)
				mDirectionalLightActive = !mDirectionalLightActive;
			break;
	}	
}

void App::mouseScroll(const double xoffset, const double yoffset)
{
	mCamera.m_fov -= static_cast<float>(yoffset);

	if (mCamera.m_fov < 1.f)
		mCamera.m_fov = 1.f;
	if (mCamera.m_fov > 45.f)
		mCamera.m_fov = 45.f;
}