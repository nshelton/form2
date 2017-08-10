
float3 calcNormal(float3 pos) {
	float2 eps = float2(thresh(length(pos - camPos)), 0.0);

	float3 nor = float3(DE(pos + eps.xyy).x - DE(pos - eps.xyy).x,
			DE(pos + eps.yxy).x - DE(pos - eps.yxy).x,
			DE(pos + eps.yyx).x - DE(pos - eps.yyx).x);
	return normalize(nor);
}

float calcAO(float3 pos, float3 nor) {
	float occ = 0.0;
	float sca = 1.0;

	for (int i = 0; i < 5; i++) {
		float hr = 0.01 / scale + 0.12 * float(i) / 4.0;
		float3 aopos = nor * hr + pos;
		float dd = DE(aopos).x;

		occ += -(dd - hr) * sca;
		sca *= 0.95;
	}
	return clamp(1.0 - 3.0 * occ, 0.0, 1.0);
}

//http://iquilezles.org/www/articles/fog/fog.htm
float3 applyFog( in float3  rgb,      // original color of the pixel
               in float distance, // camera to point distance
               in float3  rayDir)  // sun light direction
{
    float fogAmount = 1.0 - exp( -distance*_fogAmount );
    float3  fogColor  = float3(0.0, 0.0, 0.0);

    return lerp(rgb, fogColor, fogAmount );
}


//Random number [0:1] without sine
#define HASHSCALE1 .1031
float hash(float p)
{
	float3 p3  = frac((float3)p * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

float3 randomSphereDir(float2 uv)
{
    float u = nrand(uv, 10) * 2 - 1;
    float theta = nrand(uv, 11) * PI * 2;
    float u2 = sqrt(1 - u * u);
    return float3(u2 * cos(theta), u2 * sin(theta), u);
}

float3 randomHemisphereDir(float3 dir, float i)
{
	float3 v = randomSphereDir( float2(rand(i+1.), rand(i+2.)) );
	return v * sign(dot(v, dir));
}




float ambientOcclusion( in float3 p, in float3 n, in float maxDist, in float falloff )
{
	const int nbIte = 32;
    const float nbIteInv = 1.0 / float(nbIte);
    const float rad = 1.0 - 1.0 * nbIteInv; //Hemispherical factor (self occlusion correction)
    
	float ao = 0.0;
    
    for( int i=0; i<nbIte; i++ ) {
        float l = hash(float(i)) * maxDist;
        float3 rd = normalize(n+randomHemisphereDir(n, l ) * rad) * l; 
        ao += (1.0 - max(DE( p + rd ).x, 0.0)) / maxDist * falloff;
    }
	
    return clamp( 1.0 - ao * nbIteInv, 0., 1.);
}


float softshadow( float3 ro, float3 rd)
{
    float res = 1.0;
    float mint = 0.001 / scale; 
    float k = 3.0;
    float t = mint;

    for(int i = 0; i < 5; i ++ )
    {
        float h = DE(ro + rd*t).x;
        if( h < thresh(length(ro - camPos)) )
            return 0.0;
        res = min( res, k*h/t );
        t += h/scale;
    }

    return res;
}
// ao
float ao( float3 v, float3 n) {

    float step = _RenderParam.x ;

    float ao = 1.0;
    float t = 0;
    float3 pos ;

    for ( int i = 0; i < 5; i ++)
    {
        float weight = _RenderParam.y / pow(1.5, i);
        float dd = DE(t * n + v).x;
        ao -= weight * (t - dd / scale);

        t += step;
    }
	return sqrt(ao);
}

float3 light(float3 p, float2 uv, float trap, float iter)
{
	float3 nor = calcNormal(p);
	float phong = abs(dot(-nor, normalize(p - camPos)));

    float3 ray = normalize(p - camPos);
    float depth = length(p - camPos);

    // // float3 color = (float3) ambientOcclusion(p, nor, 10.0/scale , 1.);

    // // float3 color = (float3) iter;
    // // float3 color = (float3) ao(p, nor) * phong;
    
    // float3 tangent = cross(nor, cross(nor, float3(0,0,1)));
    // float3 bitangent = cross(nor, tangent);


    // float shadow = 
    //         softshadow(p, normalize(nor + tangent * _RenderParam.z)) +
    //         softshadow(p, normalize(nor - tangent * _RenderParam.z)) +
    //         softshadow(p, normalize(nor + bitangent * _RenderParam.z)) +
    //         softshadow(p, normalize(nor - bitangent * _RenderParam.z));

        
    
    float3 color = pal(pow(abs(trap) / 10 *  _RenderParam.y , _RenderParam.w )  + _RenderParam.z,
        float3(0.5, 0.5, 0.5),
        float3(0.7, 0.5, 0.5),
        float3(1.0, 1.0, 1.0),
        float3(0.0, 0.1, 0.2));

        // float3(0.5, 0.5, 0.5),
        // float3(0.5, 0.5, 0.5),
        // float3(	1.0, 1.0, 0.5),
        // float3(0.80, 0.90, 0.30));
        // color = normalize(color);

        //add palette (orbit
     color *= (1.0 - sqrt(iter)) *_RenderParam.x ;
    
    
    
    // color = (float3)shadow / 4.0;
        //(float3) softShadow(p, nor);

    color = applyFog(color, depth*scale, ray);

	return (float3) color;
}
