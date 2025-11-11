static float hash2d1d(float2 p, float seed)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * seed);
}

float2 hash3d2d(float3 p, float seed)
{
    float x = dot(p, float3(127.1, 311.7, 224.9));
    float y = dot(p, float3(192.4, 187.3, 248.6));
    return frac(sin(float2(x, y)) * seed);
}

float hash3d1d(float3 p, float seed)
{
    float x = dot(p, float3(127.1, 311.7, 224.9));
    return frac(sin(x) * seed);
}

float3 hash3d3d(float3 p, float seed)
{
    return float3(hash2d1d(p.xy, seed), hash2d1d(p.xz, seed * 123), hash2d1d(p.zy, seed * 321));
}

static float2 gradient2d(float2 ip, float seed)
{
    float h = hash2d1d(ip, seed) * 6.28318530718;
    return float2(cos(h), sin(h));
}

float3 gradient3d(float3 ip, float seed)
{
    float2 h = hash3d2d(ip, seed) * 6.28318530718;
    return normalize(float3(
        cos(h.x) * cos(h.y),
        sin(h.x),
        cos(h.x) * sin(h.y)
    ));
}

static float fade(float t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

static float3 fade(float3 t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

static float perlin2D(float2 p, float seed)
{
    float2 pi = floor(p);
    float2 pf = p - pi;

    float2 g00 = gradient2d(pi + float2(0.0, 0.0), seed);
    float2 g10 = gradient2d(pi + float2(1.0, 0.0), seed);
    float2 g01 = gradient2d(pi + float2(0.0, 1.0), seed);
    float2 g11 = gradient2d(pi + float2(1.0, 1.0), seed);

    float2 d00 = pf - float2(0.0, 0.0);
    float2 d10 = pf - float2(1.0, 0.0);
    float2 d01 = pf - float2(0.0, 1.0);
    float2 d11 = pf - float2(1.0, 1.0);

    float n00 = dot(g00, d00);
    float n10 = dot(g10, d10);
    float n01 = dot(g01, d01);
    float n11 = dot(g11, d11);

    float u = fade(pf.x);
    float v = fade(pf.y);

    float nx0 = lerp(n00, n10, u);
    float nx1 = lerp(n01, n11, u);
    float nxy = lerp(nx0, nx1, v);

    return nxy;
}

static float perlin3D(float3 p, float seed)
{
    float3 fraction = frac(p);

    float3 interpolator = fade(fraction);

    float cellNoiseZ[2];
    [unroll]
    for(int z=0;z<=1;z++)
    {
        float cellNoiseY[2];
        [unroll]
        for(int y=0;y<=1;y++)
        {
            float cellNoiseX[2];
            [unroll]
            for(int x=0;x<=1;x++)
            {
                float3 cell = floor(p) + float3(x, y, z);
                float3 cellDirection = gradient3d(cell, seed);
                float3 compareVector = fraction - float3(x, y, z);
                cellNoiseX[x] = dot(cellDirection, compareVector);
            }
            cellNoiseY[y] = lerp(cellNoiseX[0], cellNoiseX[1], interpolator.x);
        }
        cellNoiseZ[z] = lerp(cellNoiseY[0], cellNoiseY[1], interpolator.y);
    }
    float noise = lerp(cellNoiseZ[0], cellNoiseZ[1], interpolator.z);
    return noise;
}

static float fbm2D(float2 p, int octaves, float lacunarity, float persistence, float seed)
{
    float amplitude = 1.0;
    float frequency = 1.0;
    float sum = 0.0;
    float maxAmp = 0.0;

    for (int i = 0; i < octaves; ++i)
    {
        sum += perlin2D(p * frequency, seed) * amplitude;
        maxAmp += amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }

    return sum / maxAmp;
}

static float fbmPerlin3D(float3 p, int octaves, float lacunarity, float persistence, float seed)
{
    float amplitude = 1.0;
    float frequency = 1.0;
    float sum = 0.0;
    float maxAmp = 0.0;

    for (int i = 0; i < octaves; ++i)
    {
        sum += perlin3D(p * frequency, seed) * amplitude;
        maxAmp += amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }

    return sum / maxAmp;
}

static float voronoi3d(float3 value, float seed)
{
    float3 baseCell = floor(value);

    float minDistToCell = 10;
    float3 closestCell;
    [unroll]
    for(int x=-1; x<=1; x++)
    {
        [unroll]
        for(int y=-1; y<=1; y++)
        {
            [unroll]
            for(int z=-1; z<=1; z++)
            {
                float3 cell = baseCell + float3(x, y, z);
                float3 cellPosition = cell + hash3d3d(cell, seed);
                float3 toCell = cellPosition - value;
                float distToCell = length(toCell);
                if(distToCell < minDistToCell)
                {
                    minDistToCell = distToCell;
                    closestCell = cell;
                }
            }
        }
    }
    return hash3d1d(closestCell, seed);
}

static float fbmVoronoi3D(float3 p, int octaves, float lacunarity, float persistence, float seed)
{
    float amplitude = 1.0;
    float frequency = 1.0;
    float sum = 0.0;
    float maxAmp = 0.0;

    for (int i = 0; i < octaves; ++i)
    {
        sum += voronoi3d(p * frequency, seed) * amplitude;
        maxAmp += amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }

    return sum / maxAmp;
}