float2 pseudo_knightyan(float3 p)
{
    float3 CSize = float3(0.63248, 0.1,0.875);
    float DEfactor=1.;
    float orbit = 0;
    for(int i=0;i<6;i++){

        float3 start = p;
        p = 2.*clamp(p, -CSize, CSize)-p;
        float k = max(0.70968/dot(p,p),1.);
        p *= k;
        DEfactor *= k + 0.05;

        orbit += length(start - p);
    }
    float rxy=length(p.xy);
    float ds =  max(rxy-0.92784, abs(rxy*p.z) / length(p))/DEfactor;

    return float2(ds,  orbit);
}

float tglad_variant(float3 z0)
{
    z0 = modc(z0, 2.0);
    float mr=0.25, mxr=1.25;

    float4 scale = (float)(-2) , p0=_FractalA;
    float4 z = float4(z0,1.0);
    for (int n = 0; n < 8; n++) {
        z.xyz=clamp(z.xyz, -0.94, 0.94)*2.0-z.xyz;
        z*=scale/clamp(dot(z.xyz,z.xyz),mr,mxr);
        z+=p0;
    }
    float dS=(length(max(abs(z.xyz)-float3(1.2,49,1.4),0.0))-0.06)/z.w;
    return dS;
}


float2 tglad(float3 z0)
{
    // z0 = modc(z0, 2.0);

    float mr=0.25, mxr=1.0;
    float4 scale=float4(-3.12,-3.12,-3.12,3.12), p0=_FractalA;
    float4 z = float4(z0,1.0);
    float orbit = 0;

    for (int n = 0; n < 8; n++) {
        float3 start = z.xyz;

        z.xyz=clamp(z.xyz, -_FractalA.xyz, _FractalA.xyz)*2.0-z.xyz;
        z*=scale/clamp(dot(z.xyz,z.xyz),mr,mxr);
        z+=p0;
        orbit += length(start-z.xyz);
        

    }

    float dS=(length(max(abs(z.xyz)-float3(1.2,49.0,1.4),0.0))-0.06)/z.w;
    return float2(dS, orbit );
}

// distance function from Hartverdrahtet
// ( http://www.pouet.net/prod.php?which=59086 )
float2 hartverdrahtet(float3 f)
{
    float3 cs= _FractalA.xyz;
    float fs=_FractalA.w;
    float3 fc=0;
    float fu=10.;
    float fd=.763;
    float orbit = 0.0;
    // scene selection
    {
        int i = 8;
        if(i==0) cs.y=.58;
        if(i==1) cs.xy=.5;
        if(i==2) cs.xy=.5;
        if(i==3) fu=1.01,cs.x=.9;
        if(i==4) fu=1.01,cs.x=.9;
        if(i==6) cs=float3(.5,.5,1.04);
        if(i==5) fu=.9;
        if(i==7) fd=.7,fs=1.34,cs.xy=.5;
        if(i==8) fc.z=-.38;
    }
    
    float v=1.;
    for(int i=0; i<12; i++){
        float3 start = f;

        f=2.*clamp(f,-cs,cs)-f;
        float c=max(fs/dot(f,f),1.);
        f*=c;
        v*=c;
        f+=fc;

        orbit += length(start-f);
    }
    float z=length(f.xy)-fu;
    float d =  fd*max(z,abs(length(f.xy)*f.z)/sqrt(dot(f,f)))/abs(v);

    return float2(d, orbit);
}

float2 DE(float3 d)
{
	return tglad(d);
}