#version 440 core

in vec2 SCREEN;
in float TIME;

out vec4 color;

in vec3 cam_pos;
in vec3 cam_forward;
in vec3 cam_up;
in vec3 cam_right;

struct Plane
{
  vec3 Pos;
  vec3 Up;
};

struct Sphere
{
	vec3 Pos;
	float RSquared;
};

bool IntersectsPlane(vec3 ray_origin, vec3 ray_dir, Plane p, out vec3 hitpos, out vec3 Normal) 
{
    float denom = dot(p.Up, ray_dir);
    if (denom < 0)
    {
        float dist = dot(p.Pos - ray_origin, p.Up) / denom;
        if (dist >= 0)
		{
			hitpos = ray_origin + dist * ray_dir;
			vec3 dr = hitpos - p.Pos;
			if (dot(dr, dr) > 900) return false;
			return true;
		}
    }
    return false;
} 

/*
bool IntersectsSphere()
{
	Vector3 L = s.Position - r.Origin;
    float tA = Vector3.Dot(L, r.Direction);

    float dR = s.RSquared - L.LengthSquared();

    float discr = dR + tA * tA;
    if (discr < 0) return null;

    float tB = FMath.Sqrt(discr);

    float dist;
    bool Inside = dR > 0;
    if (Inside) dist = tA + tB;
    else
    {
        dist = tA - tB;
        if (dist < 0) return null;
    }

    Vector3 hitp = r.Walk(dist);
    Vector3 Normal = Inside ? Vector3.Normalize(s.Position - hitp) : Vector3.Normalize(hitp - s.Position);

    return new HitInfo(s, hitp, Normal, dist, Inside);
}
*/

void main(void)
{
	float X = gl_FragCoord.x / SCREEN.x;
	float Y = gl_FragCoord.y / SCREEN.y;

	float xnorm = (X - 0.5) * 2;
	float ynorm = (Y - 0.5) * 2;

	vec3 ray_origin = cam_pos;
	vec3 ray_dir = normalize(cam_forward + (xnorm * cam_right) + (ynorm * cam_up));
	
	Plane p = Plane(vec3(0,0,0), vec3(0,1,0));
	
    vec3 hitpoint = vec3(0,0,0);
	vec3 normal = vec3(0,0,0);

	//Plane
	if (IntersectsPlane(ray_origin, ray_dir, p, hitpoint, normal))
	{

		int hitx = int(floor(hitpoint.x));
		int hitz = int(floor(hitpoint.z));
		int hitsum = (hitx + hitz) & 1;

		if (hitsum==1) color = vec4(1,1,1,1);
		else color = vec4(0,0,0,0);
		return;
	}

	//Sky
	color = vec4(149/255.0,200/255.0,216/255.0,0) * (dot(normalize(vec3(1,1,1)), ray_dir) + 1) / 2;
	return;
}