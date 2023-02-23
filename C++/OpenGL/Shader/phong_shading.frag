#version 450 core

layout (location = 0) out vec4 fragmentColor;

in vec4 position;
in vec3 normal;
in vec2 uv;

struct PhongMaterial
{
	vec3 kS;
	vec3 kD; // Solid Color
	float shininess;
};

uniform PhongMaterial material;

struct SpotLight
{
	vec3 position;
	vec3 color;
	vec3 spotDirection;
	float cutOffAngle;
};

uniform SpotLight spotLight;
uniform bool spotLightIsActive = true;
uniform vec3 ambientLightColor = vec3(1.0);
uniform vec3 cameraPosition = vec3(1.0);
uniform int pattern = 0;

float getTreePattern(in vec2 lookup, in vec3 color)
{
	vec2 m = 2.0 * fract(vec2(lookup.x * 10.0, lookup.y * 5.0)) - vec2(1.0);
	float a = atan(m.y, m.x);
	float f = clamp(sin(50.0 * (sqrt(length(m)) - 0.02 * a - 0.3)), 0.0, 1.0);

	return f;
}

float getStripePattern(in vec2 lookup, in vec3 color)
{
	vec2 xy = abs(fract(50.0 * lookup) * 2.0 - 1.0);
	float f = mod(xy.y, 2.0);
	return f;
}

void main()
{
	vec3 directionalLight = normalize(vec3(-1.0, -1.0, -1.0)); // L, sun
	vec3 L = directionalLight * -1.0; // For shading calculations

	vec3 N = normalize(normal);
	vec3 V = normalize(cameraPosition - position.xyz);
	vec3 R = reflect(-L, N);
	float cosTheta = max(dot(L, N), 0.0);
	
	float p = 1.0;
	if (pattern == 1)
		p = getTreePattern(uv, material.kD);
	else if (pattern == 2)
		p = getStripePattern(uv, material.kD);

	vec3 kD = p * material.kD;
	vec3 kS = (1.0 - p) * material.kS;
	vec3 dirDiffuse = cosTheta * kD * spotLight.color;	
	vec3 ambient = kD * ambientLightColor;
	vec3 dirSpecular = cosTheta * kS * spotLight.color * pow(max(dot(R, V), 0.0), material.shininess);

	// Calculate values for spotlight
	L = normalize(spotLight.position - position.xyz);
	R = reflect(-L, N);
	cosTheta = max(dot(L, N), 0.0);
	vec3 spotDiffuse = cosTheta * kD * spotLight.color;	
	vec3 spotSpecular = cosTheta * kS * spotLight.color * pow(max(dot(R, V), 0.0), material.shininess);

	// Check if fragment is within spotlight
	vec3 fragVec = normalize(position.xyz - spotLight.position);
	float fragmentLightAngle = dot(fragVec, normalize(spotLight.spotDirection));

	if (fragmentLightAngle > spotLight.cutOffAngle && spotLightIsActive)
		fragmentColor = vec4(ambient + dirDiffuse + spotDiffuse + dirSpecular + spotSpecular, 1.0);
	else
		fragmentColor = vec4((ambient + dirDiffuse + dirSpecular) * 0.3, 1.0);
}
