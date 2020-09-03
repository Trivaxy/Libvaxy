/*
	------------------------------------------------------------------------------
		Licensing information can be found at the end of the file.
	------------------------------------------------------------------------------

	cute_c2.h - v1.09

	To create implementation (the function definitions)
		#define CUTE_C2_IMPLEMENTATION
	in *one* C/CPP file (translation unit) that includes this file


	SUMMARY

		cute_c2 is a single-file header that implements 2D collision detection routines
		that test for overlap, and optionally can find the collision manifold. The
		manifold contains all necessary information to prevent shapes from inter-
		penetrating, which is useful for character controllers, general physics
		simulation, and user-interface programming.

		This header implements a group of "immediate mode" functions that should be
		very easily adapted into pre-existing projects.


	THE IMPORTANT PARTS

		Most of the math types in this header are for internal use. Users care about
		the shape types and the collision functions.

		SHAPE TYPES:
		* c2Circle
		* c2Capsule
		* c2AABB
		* c2Ray
		* c2Poly

		COLLISION FUNCTIONS (*** is a shape name from the above list):
		* c2***to***         - boolean YES/NO hittest
		* c2***to***Manifold - construct manifold to describe how shapes hit
		* c2GJK              - runs GJK algorithm to find closest point pair between two shapes
		* c2TOI              - computes the time of impact between two shapes, useful for sweeping shapes, or doing shape casts
		* c2MakePoly         - Runs convex hull algorithm and computes normals on input point-set
		* c2Collided         - generic version of c2***to*** funcs
		* c2Collide          - generic version of c2***to***Manifold funcs
		* c2CastRay          - generic version of c2Rayto*** funcs

		The rest of the header is more or less for internal use. Here is an example of
		making some shapes and testing for collision:

			c2Circle c;
			c.p = position;
			c.r = radius;

			c2Capsule cap;
			cap.a = first_endpoint;
			cap.b = second_endpoint;
			cap.r = radius;

			int hit = c2CircletoCapsule(c, cap);
			if (hit)
			{
				handle collision here...
			}
	
		For more code examples and tests please see:
		https://github.com/RandyGaul/cute_header/tree/master/examples_cute_gl_and_c2

		Here is a past discussion thread on this header:
		https://www.reddit.com/r/gamedev/comments/5tqyey/tinyc2_2d_collision_detection_library_in_c/

		Here is a very nice repo containing various tests and examples using SFML for rendering:
		https://github.com/sro5h/tinyc2-tests


	FEATURES

		* Circles, capsules, AABBs, rays and convex polygons are supported
		* Fast boolean only result functions (hit yes/no)
		* Slghtly slower manifold generation for collision normals + depths +points
		* GJK implementation (finds closest points for disjoint pairs of shapes)
		* Shape casts/sweeps with c2TOI function (time of impact)
		* Robust 2D convex hull generator
		* Lots of correctly implemented and tested 2D math routines
		* Implemented in portable C, and is readily portable to other languages
		* Generic c2Collide, c2Collided and c2CastRay function (can pass in any shape type)
		* Extensive examples at: https://github.com/RandyGaul/cute_headers/tree/master/examples_cute_gl_and_c2


	Revision History
	
		1.0  (02/13/2017) initial release
		1.01 (02/13/2017) const crusade, minor optimizations, capsule degen
		1.02 (03/21/2017) compile fixes for c on more compilers
		1.03 (09/15/2017) various bugfixes and quality of life changes to manifolds
		1.04 (03/25/2018) fixed manifold bug in c2CircletoAABBManifold
		1.05 (11/01/2018) added c2TOI (time of impact) for shape cast/sweep test
		1.06 (08/23/2019) C2_*** types to C2_TYPE_***, and CUTE_C2_API
		1.07 (10/19/2019) Optimizations to c2TOI - breaking change to c2GJK API
		1.08 (12/22/2019) Remove contact point + normal from c2TOI, removed feather
		                  radius from c2GJK, fixed various bugs in capsule to poly
		                  manifold, did a pass on all docs
		1.09 (07/27/2019) Added c2Inflate - to inflate/deflate shapes for c2TOI


	Contributors

		Plastburk         1.01 - const pointers pull request
		mmozeiko          1.02 - 3 compile bugfixes
		felipefs          1.02 - 3 compile bugfixes
		seemk             1.02 - fix branching bug in c2Collide
		sro5h             1.02 - bug reports for multiple manifold funcs
		sro5h             1.03 - work involving quality of life fixes for manifolds
		Wizzard033        1.06 - C2_*** types to C2_TYPE_***, and CUTE_C2_API
		Tyler Glaeil      1.08 - Lots of bug reports and disussion on capsules + TOIs


	DETAILS/ADVICE

		BROAD PHASE

			This header does not implement a broad-phase, and instead concerns itself with
			the narrow-phase. This means this header just checks to see if two individual
			shapes are touching, and can give information about how they are touching.

			Very common 2D broad-phases are tree and grid approaches. Quad trees are good
			for static geometry that does not move much if at all. Dynamic AABB trees are
			good for general purpose use, and can handle moving objects very well. Grids
			are great and are similar to quad trees.

			If implementing a grid it can be wise to have each collideable grid cell hold
			an integer. This integer refers to a 2D shape that can be passed into the
			various functions in this header. The shape can be transformed from "model"
			space to "world" space using c2x -- a transform struct. In this way a grid
			can be implemented that holds any kind of convex shape (that this header
			supports) while conserving memory with shape instancing.

		NUMERIC ROBUSTNESS

			Many of the functions in cute c2 use `c2GJK`, an implementation of the GJK
			algorithm. Internally GJK computes signed area values, and these values are
			very numerically sensitive to large shapes. This means the GJK function will
			break down if input shapes are too large or too far away from the origin.

			In general it is best to compute collision detection on small shapes very
			close to the origin. One trick is to keep your collision information numerically
			very tiny, and simply scale it up when rendering to the appropriate size.

			For reference, if your shapes are all AABBs and contain a width and height
			of somewhere between 1.0f and 10.0f, everything will be fine. However, once
			your shapes start approaching a width/height of 100.0f to 1000.0f GJK can
			start breaking down.

			This is a complicated topic, so feel free to ask the author for advice here.

			Here is an example demonstrating this problem with two large AABBs:
			https://github.com/RandyGaul/cute_headers/issues/160

		Please email at my address with any questions or comments at:
		author's last name followed by 1748 at gmail
*/

using System;

namespace Libvaxy.Collision
{
	// 2d vector
	public class Vector
	{
		public float X;
		public float Y;

		public Vector()
		{
			X = 0;
			Y = 0;
		}

		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}
	}

	// 2d rotation composed of cos/sin pair
	public class Rotation
	{
		public float Cos;
		public float Sin;
	}

	// 2d rotation matrix
	public class RotationMatrix
	{
		public Vector X;
		public Vector Y;

		public RotationMatrix()
		{
			X = new Vector();
			Y = new Vector();
		}

		public RotationMatrix(Vector x, Vector y)
		{
			X = x;
			Y = y;
		}
	}

	// 2d transformation "x"
	// These are used especially for c2Poly when a c2Poly is passed to a function.
	// Since polygons are prime for "instancing" a c2x transform can be used to
	// transform a polygon from local space to world space. In functions that take
	// a c2x pointer (like c2PolytoPoly), these pointers can be NULL, which represents
	// an identity transformation and assumes the verts inside of c2Poly are already
	// in world space.

	public class Transformation
	{
		public Vector Position;
		public Rotation Rotation;

		public Transformation()
		{
			Position = new Vector();
			Rotation = new Rotation();
		}

		public Transformation(Vector p, Rotation r)
		{
			this.Position = p;
			this.Rotation = r;
		}
	}

	// 2d halfspace (aka plane, aka line)
	public class HalfSpace
	{
		public Vector Normal;   // normal, normalized
		public float DistanceFromOrigin; // distance to origin from plane, or ax + by = d

		public HalfSpace()
		{
			Normal = new Vector();
			DistanceFromOrigin = 0;
		}

		public HalfSpace(Vector n, float d)
		{
			this.Normal = n;
			this.DistanceFromOrigin = d;
		}
	}

	public class Circle
	{
		public Vector Position;
		public float Radius;

		public Circle()
		{
			Position = new Vector();
			Radius = 0;
		}

		public Circle(Vector p, float r)
		{
			this.Position = p;
			this.Radius = r;
		}
	}

	public class AABB
	{
		public Vector Min;
		public Vector Max;

		public AABB()
		{
			Min = new Vector();
			Max = new Vector();
		}

		public AABB(Vector min, Vector max)
		{
			this.Min = min;
			this.Max = max;
		}
	}

	// a capsule is defined as a line segment (from a to b) and radius r
	public class Capsule
	{
		public Vector Start;
		public Vector End;
		public float Radius;

		public Capsule()
		{
			Start = new Vector();
			End = new Vector();
			Radius = 0;
		}
	}

	public class Poly
	{
		public int VertexCount;
		public Vector[] Vertices; // max size 8
		public Vector[] Normals; // max size 8

		public Poly()
		{
			VertexCount = 0;
			Vertices = new Vector[8];
			Normals = new Vector[8];

			for (int i = 0; i < 8; i++)
			{
				Vertices[i] = new Vector();
				Normals[i] = new Vector();
			}
		}
	}

	// IMPORTANT:
	// Many algorithms in this file are sensitive to the magnitude of the
	// ray direction (c2Ray::d). It is highly recommended to normalize the
	// ray direction and use t to specify a distance. Please see this link
	// for an in-depth explanation: https://github.com/RandyGaul/cute_headers/issues/30
	public class Ray
	{
		public Vector Position;   // position
		public Vector Direction;   // direction (normalized)
		public float Length; // distance along d from position p to find endpoint of ray

		public Ray()
		{
			Position = new Vector();
			Direction = new Vector();
			Length = 0;
		}

		public Ray(Vector p, Vector d, float t)
		{
			this.Position = p;
			this.Direction = d;
			this.Length = t;
		}
	}

	public class RayCast
	{
		public float TimeOfImpact; // time of impact
		public Vector ImpactNormal;   // normal of surface at impact (unit length)

		public RayCast()
		{
			TimeOfImpact = 0;
			ImpactNormal = new Vector();
		}

		public RayCast(float t, Vector n)
		{
			this.TimeOfImpact = t;
			this.ImpactNormal = n;
		}
	}

	// contains all information necessary to resolve a collision, or in other words
	// this is the information needed to separate shapes that are colliding. Doing
	// the resolution step is *not* included in cute_c2.
	public class Manifold
	{
		public int Count;
		public float[] Depths; // max size 2
		public Vector[] ContactPoints; // max size 2

		// always points from shape A to shape B (first and second shapes passed into
		// any of the c2***to***Manifold functions)
		public Vector Direction;

		public Manifold()
		{
			Count = 0;
			Depths = new float[2];
			ContactPoints = new[] { new Vector(), new Vector() };
		}
	}

	public enum ShapeType
	{
		None,
		Circle,
		AABB,
		Capsule,
		Poly
	}

	// This struct is only for advanced usage of the c2GJK function. See comments inside of the
	// c2GJK function for more details.
	public class GJKCache
	{
		public float Metric;
		public int Count;
		public int[] IA;  // max size 3
		public int[] IB;  // max size 3
		public float Div;

		public GJKCache()
		{
			Metric = 0;
			Count = 0;
			IA = new int[3];
			IB = new int[3];
			Div = 0;
		}
	}

	public class DynamicCollision
	{
		public const int MaxPolygonVertices = 8;
		public const int GJKIterations = 20;

		// This is an advanced function, intended to be used by people who know what they're doing.
		//
		// Runs the GJK algorithm to find closest points, returns distance between closest points.
		// outA and outB can be NULL, in this case only distance is returned. ax_ptr and bx_ptr
		// can be NULL, and represent local to world transformations for shapes A and B respectively.
		// use_radius will apply radii for capsules and circles (if set to false, spheres are
		// treated as points and capsules are treated as line segments i.e. rays). The cache parameter
		// should be NULL, as it is only for advanced usage (unless you know what you're doing, then
		// go ahead and use it). iterations is an optional parameter.
		//
		// IMPORTANT NOTE:
		// The GJK function is sensitive to large shapes, since it internally will compute signed area
		// values. `c2GJK` is called throughout cute c2 in many ways, so try to make sure all of your
		// collision shapes are not gigantic. For example, try to keep the volume of all your shapes
		// less than 100.0f. If you need large shapes, you should use tiny collision geometry for all
		// cute c2 function, and simply render the geometry larger on-screen by scaling it up.
		// float c2GJK(const void* A, C2_TYPE typeA, const c2x* ax_ptr, const void* B, C2_TYPE typeB, const c2x* bx_ptr, c2v* outA, c2v* outB, int use_radius, int* iterations, c2GJKCache* cache);

		// This is an advanced function, intended to be used by people who know what they're doing.
		//
		// Computes the time of impact from shape A and shape B. The velocity of each shape is provided
		// by vA and vB respectively. The shapes are *not* allowed to rotate over time. The velocity is
		// assumed to represent the change in motion from time 0 to time 1, and so the return value will
		// be a number from 0 to 1. To move each shape to the colliding configuration, multiply vA and vB
		// each by the return value. ax_ptr and bx_ptr are optional parameters to transforms for each shape,
		// and are typically used for polygon shapes to transform from model to world space. Set these to
		// NULL to represent identity transforms. iterations is an optional parameter. use_radius
		// will apply radii for capsules and circles (if set to false, spheres are treated as points and
		// capsules are treated as line segments i.e. rays).
		//
		// IMPORTANT NOTE:
		// The c2TOI function can be used to implement a "swept character controller", but it can be
		// difficult to do so. Say we compute a time of impact with `c2TOI` and move the shapes to the
		// time of impact, and adjust the velocity by zeroing out the velocity along the surface normal.
		// If we then call `c2TOI` again, it will fail since the shapes will be considered to start in
		// a colliding configuration. There are many styles of tricks to get around this problem, and
		// all of them involve giving the next call to `c2TOI` some breathing room. It is recommended
		// to use some variation of the following algorithm:
		//
		// 1. Call c2TOI.
		// 2. Move the shapes to the TOI.
		// 3. Slightly inflate the size of one, or both, of the shapes so they will be intersecting.
		//    The purpose is to make the shapes numerically intersecting, but not visually intersecting.
		//    Another option is to call c2TOI with slightly deflated shapes.
		//    See the function `c2Inflate` for some more details.
		// 4. Compute the collision manifold between the inflated shapes (for example, use c2PolytoPolyManifold).
		// 5. Gently push the shapes apart. This will give the next call to c2TOI some breathing room.
		// float c2TOI(const void* A, C2_TYPE typeA, const c2x* ax_ptr, c2v vA, const void* B, C2_TYPE typeB, const c2x* bx_ptr, c2v vB, int use_radius, int* iterations);

		// Inflating a shape.
		//
		// This is useful to numerically grow or shrink a polytope. For example, when calling
		// a time of impact function it can be good to use a slightly smaller shape. Then, once
		// both shapes are moved to the time of impact a collision manifold can be made from the
		// slightly larger (and now overlapping) shapes.
		//
		// IMPORTANT NOTE
		// Inflating a shape with sharp corners can cause those corners to move dramatically.
		// Deflating a shape can avoid this problem, but deflating a very small shape can invert
		// the planes and result in something that is no longer convex. Make sure to pick an
		// appropriately small skin factor, for example 1.0e-6f.
		// void c2Inflate(void* shape, C2_TYPE type, float skin_factor);

		// Computes 2D convex hull. Will not do anything if less than two verts supplied. If
		// more than C2_MAX_POLYGON_VERTS are supplied extras are ignored.
		// int c2Hull(c2v* verts, int count);
		// void c2Norms(c2v* verts, c2v* norms, int count);

		// runs c2Hull and c2Norms, assumes p->verts and p->count are both set to valid values
		// void c2MakePoly(c2Poly* p);

		// Generic collision detection routines, useful for games that want to use some poly-
		// morphism to write more generic-styled code. Internally calls various above functions.
		// For AABBs/Circles/Capsules ax and bx are ignored. For polys ax and bx can define
		// model to world transformations (for polys only), or be NULL for identity transforms.
		// int c2Collided(const void* A, const c2x* ax, C2_TYPE typeA, const void* B, const c2x* bx, C2_TYPE typeB);
		// void c2Collide(const void* A, const c2x* ax, C2_TYPE typeA, const void* B, const c2x* bx, C2_TYPE typeB, c2Manifold* m);
		// int c2CastRay(c2Ray A, const void* B, const c2x* bx, C2_TYPE typeB, c2Raycast* out);

		// adjust these primitives as seen fit
		/* inline */
		static void c2SinCos(float radians, ref float s, ref float c) { c = (float)Math.Cos(radians); s = (float)Math.Sin(radians); }

		// The rest of the functions in the header-only portion are all for internal use
		// and use the author's personal naming conventions. It is recommended to use one's
		// own math library instead of the one embedded here in cute_c2, but for those
		// curious or interested in trying it out here's the details:

		// The Mul functions are used to perform multiplication. x stands for transform,
		// v stands for vector, s stands for scalar, r stands for rotation, h stands for
		// halfspace and T stands for transpose.For example c2MulxvT stands for "multiply
		// a transform with a vector, and transpose the transform".

		// vector ops
		/* inline */
		static Vector c2V(float x, float y) { Vector a = new Vector(); a.X = x; a.Y = y; return a; }

		/* inline */
		static Vector c2Add(Vector a, Vector b) { a.X += b.X; a.Y += b.Y; return a; }

		/* inline */
		static Vector c2Sub(Vector a, Vector b) { Vector c = new Vector(a.X - b.X, a.Y - b.Y); return c; }

		/* inline */
		static float c2Dot(Vector a, Vector b) { return a.X * b.X + a.Y * b.Y; }

		/* inline */
		static Vector c2Mulvs(Vector a, float b) { a.X *= b; a.Y *= b; return a; }

		/* inline */
		static Vector c2Mulvv(Vector a, Vector b) { a.X *= b.X; a.Y *= b.Y; return a; }

		/* inline */
		static Vector c2Div(Vector a, float b) { return c2Mulvs(a, 1.0f / b); }

		/* inline */
		static Vector c2Skew(Vector a) { Vector b = new Vector(); b.X = -a.Y; b.Y = a.X; return b; }

		/* inline */
		static Vector c2CCW90(Vector a) { Vector b = new Vector(); b.X = a.Y; b.Y = -a.X; return b; }

		/* inline */
		static float c2Det2(Vector a, Vector b) { return a.X * b.Y - a.Y * b.X; }
		/* inline */
		static Vector c2Minv(Vector a, Vector b) { return c2V(a.X < b.X ? a.X : b.X, a.Y < b.Y ? a.Y : b.Y); }

		/* inline */
		static Vector c2Maxv(Vector a, Vector b) { return c2V(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y)); }

		/* inline */
		static Vector c2Clampv(Vector a, Vector lo, Vector hi) { return c2Maxv(lo, c2Minv(a, hi)); }

		/* inline */
		static Vector c2Absv(Vector a) { return c2V(Math.Abs(a.X), Math.Abs(a.Y)); }

		/* inline */
		static float c2Hmin(Vector a) { return a.X < a.Y ? a.X : a.Y; }

		/* inline */
		static float c2Hmax(Vector a) { return Math.Max(a.X, a.Y); }

		/* inline */
		static float c2Len(Vector a) { return (float)Math.Sqrt(c2Dot(a, a)); }

		/* inline */
		static Vector c2Norm(Vector a) { return c2Div(a, c2Len(a)); }

		/* inline */
		static Vector c2SafeNorm(Vector a) { float sq = c2Dot(a, a); return sq != 0 ? c2Div(a, c2Len(a)) : c2V(0, 0); }

		/* inline */
		static Vector c2Neg(Vector a) { return c2V(-a.X, -a.Y); }

		/* inline */
		static Vector c2Lerp(Vector a, Vector b, float t) { return c2Add(a, c2Mulvs(c2Sub(b, a), t)); }

		/* inline */
		static int c2Parallel(Vector a, Vector b, float kTol)
		{
			float k = c2Len(a) / c2Len(b);
			b = c2Mulvs(b, k);
			if (Math.Abs(a.X - b.X) < kTol && Math.Abs(a.Y - b.Y) < kTol) return 1;
			return 0;
		}

		// rotation ops
		/* inline */
		static Rotation c2Rot(float radians) { Rotation r = new Rotation(); c2SinCos(radians, ref r.Sin, ref r.Cos); return r; }

		/* inline */
		static Rotation c2RotIdentity() { Rotation r = new Rotation(); r.Cos = 1.0f; r.Sin = 0; return r; }

		/* inline */
		static Vector c2RotX(Rotation r) { return c2V(r.Cos, r.Sin); }

		/* inline */
		static Vector c2RotY(Rotation r) { return c2V(-r.Sin, r.Cos); }

		/* inline */
		static Vector c2Mulrv(Rotation a, Vector b) { return c2V(a.Cos * b.X - a.Sin * b.Y, a.Sin * b.X + a.Cos * b.Y); }

		/* inline */
		static Vector c2MulrvT(Rotation a, Vector b) { return c2V(a.Cos * b.X + a.Sin * b.Y, -a.Sin * b.X + a.Cos * b.Y); }

		/* inline */
		static Rotation c2Mulrr(Rotation a, Rotation b) { Rotation c = new Rotation(); c.Cos = a.Cos * b.Cos - a.Sin * b.Sin; c.Sin = a.Sin * b.Cos + a.Cos * b.Sin; return c; }

		/* inline */
		static Rotation c2MulrrT(Rotation a, Rotation b) { Rotation c = new Rotation(); c.Cos = a.Cos * b.Cos + a.Sin * b.Sin; c.Sin = a.Cos * b.Sin - a.Sin * b.Cos; return c; }

		/* inline */
		static Vector c2Mulmv(RotationMatrix a, Vector b) { Vector c = new Vector(); c.X = a.X.X * b.X + a.Y.X * b.Y; c.Y = a.X.Y * b.X + a.Y.Y * b.Y; return c; }

		/* inline */
		static Vector c2MulmvT(RotationMatrix a, Vector b) { Vector c = new Vector(); c.X = a.X.X * b.X + a.X.Y * b.Y; c.Y = a.Y.X * b.X + a.Y.Y * b.Y; return c; }

		/* inline */
		static RotationMatrix c2Mulmm(RotationMatrix a, RotationMatrix b) { RotationMatrix c = new RotationMatrix(); c.X = c2Mulmv(a, b.X); c.Y = c2Mulmv(a, b.Y); return c; }

		/* inline */
		static RotationMatrix c2MulmmT(RotationMatrix a, RotationMatrix b) { RotationMatrix c = new RotationMatrix(); c.X = c2MulmvT(a, b.X); c.Y = c2MulmvT(a, b.Y); return c; }

		// transform ops
		/* inline */
		static Transformation c2xIdentity() { Transformation x = new Transformation(); x.Position = c2V(0, 0); x.Rotation = c2RotIdentity(); return x; }

		/* inline */
		static Vector c2Mulxv(Transformation a, Vector b) { return c2Add(c2Mulrv(a.Rotation, b), a.Position); }

		/* inline */
		static Vector c2MulxvT(Transformation a, Vector b) { return c2MulrvT(a.Rotation, c2Sub(b, a.Position)); }

		/* inline */
		static Transformation c2Mulxx(Transformation a, Transformation b) { Transformation c = new Transformation(); c.Rotation = c2Mulrr(a.Rotation, b.Rotation); c.Position = c2Add(c2Mulrv(a.Rotation, b.Position), a.Position); return c; }

		/* inline */
		static Transformation c2MulxxT(Transformation a, Transformation b) { Transformation c = new Transformation(); c.Rotation = c2MulrrT(a.Rotation, b.Rotation); c.Position = c2MulrvT(a.Rotation, c2Sub(b.Position, a.Position)); return c; }

		/* inline */
		static Transformation c2Transform(Vector p, float radians) { Transformation x = new Transformation(); x.Rotation = c2Rot(radians); x.Position = p; return x; }

		// halfspace ops
		/* inline */
		static Vector c2Origin(HalfSpace h) { return c2Mulvs(h.Normal, h.DistanceFromOrigin); }

		/* inline */
		static float c2Dist(HalfSpace h, Vector p) { return c2Dot(h.Normal, p) - h.DistanceFromOrigin; }

		/* inline */
		static Vector c2Project(HalfSpace h, Vector p) { return c2Sub(p, c2Mulvs(h.Normal, c2Dist(h, p))); }

		/* inline */
		static HalfSpace c2Mulxh(Transformation a, HalfSpace b) { HalfSpace c = new HalfSpace(); c.Normal = c2Mulrv(a.Rotation, b.Normal); c.DistanceFromOrigin = c2Dot(c2Mulxv(a, c2Origin(b)), c.Normal); return c; }

		/* inline */
		static HalfSpace c2MulxhT(Transformation a, HalfSpace b) { HalfSpace c = new HalfSpace(); c.Normal = c2MulrvT(a.Rotation, b.Normal); c.DistanceFromOrigin = c2Dot(c2MulxvT(a, c2Origin(b)), c.Normal); return c; }

		/* inline */
		static Vector c2Intersect(Vector a, Vector b, float da, float db) { return c2Add(a, c2Mulvs(c2Sub(b, a), (da / (da - db)))); }

		/* inline */
		static void c2BBVerts(Vector[] verts, AABB bb)
		{
			verts[0] = bb.Min;
			verts[1] = c2V(bb.Max.X, bb.Min.Y);
			verts[2] = bb.Max;
			verts[3] = c2V(bb.Min.X, bb.Max.Y);
		}

		public static bool c2Collided(object A, Transformation ax, ShapeType typeA, object B, Transformation bx, ShapeType typeB)
		{
			switch (typeA)
			{
				case ShapeType.Circle:
					switch (typeB)
					{
						case ShapeType.Circle: return c2CircletoCircle((Circle)A, (Circle)B);
						case ShapeType.AABB: return c2CircletoAABB((Circle)A, (AABB)B);
						case ShapeType.Capsule: return c2CircletoCapsule((Circle)A, (Capsule)B);
						case ShapeType.Poly: return c2CircletoPoly((Circle)A, (Poly)B, bx);
						default: return false;
					}

				case ShapeType.AABB:
					switch (typeB)
					{
						case ShapeType.Circle: return c2CircletoAABB((Circle)B, (AABB)A);
						case ShapeType.AABB: return c2AABBtoAABB((AABB)A, (AABB)B);
						case ShapeType.Capsule: return c2AABBtoCapsule((AABB)A, (Capsule)B);
						case ShapeType.Poly: return c2AABBtoPoly((AABB)A, (Poly)B, bx);
						default: return false;
					}

				case ShapeType.Capsule:
					switch (typeB)
					{
						case ShapeType.Circle: return c2CircletoCapsule((Circle)B, (Capsule)A);
						case ShapeType.AABB: return c2AABBtoCapsule((AABB)B, (Capsule)A);
						case ShapeType.Capsule: return c2CapsuletoCapsule((Capsule)A, (Capsule)B);
						case ShapeType.Poly: return c2CapsuletoPoly((Capsule)A, (Poly)B, bx);
						default: return false;
					}

				case ShapeType.Poly:
					switch (typeB)
					{
						case ShapeType.Circle: return c2CircletoPoly((Circle)B, (Poly)A, ax);
						case ShapeType.AABB: return c2AABBtoPoly((AABB)B, (Poly)A, ax);
						case ShapeType.Capsule: return c2CapsuletoPoly((Capsule)B, (Poly)A, ax);
						case ShapeType.Poly: return c2PolytoPoly((Poly)A, ax, (Poly)B, bx);
						default: return false;
					}

				default:
					return false;
			}
		}

		public static Manifold c2Collide(in object A, Transformation ax, ShapeType typeA, in object B, Transformation bx, ShapeType typeB)
		{
			Manifold m;

			switch (typeA)
			{
				case ShapeType.Circle:
					switch (typeB)
					{
						case ShapeType.Circle: return c2CircletoCircleManifold((Circle)A, (Circle)B);
						case ShapeType.AABB: return c2CircletoAABBManifold((Circle)A, (AABB)B);
						case ShapeType.Capsule: return c2CircletoCapsuleManifold((Circle)A, (Capsule)B);
						case ShapeType.Poly: return c2CircletoPolyManifold((Circle)A, (Poly)B, bx);
					}
					break;

				case ShapeType.AABB:
					switch (typeB)
					{
						case ShapeType.Circle: m = c2CircletoAABBManifold((Circle)B, (AABB)A); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.AABB: return c2AABBtoAABBManifold((AABB)A, (AABB)B);
						case ShapeType.Capsule: return c2AABBtoCapsuleManifold((AABB)A, (Capsule)B);
						case ShapeType.Poly: return c2AABBtoPolyManifold((AABB)A, (Poly)B, bx);
					}
					break;

				case ShapeType.Capsule:
					switch (typeB)
					{
						case ShapeType.Circle: m = c2CircletoCapsuleManifold((Circle)B, (Capsule)A); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.AABB: m = c2AABBtoCapsuleManifold((AABB)B, (Capsule)A); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.Capsule: return c2CapsuletoCapsuleManifold((Capsule)A, (Capsule)B);
						case ShapeType.Poly: return c2CapsuletoPolyManifold((Capsule)A, (Poly)B, bx);
					}
					break;

				case ShapeType.Poly:
					switch (typeB)
					{
						case ShapeType.Circle: m = c2CircletoPolyManifold((Circle)B, (Poly)A, ax); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.AABB: m = c2AABBtoPolyManifold((AABB)B, (Poly)A, ax); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.Capsule: m = c2CapsuletoPolyManifold((Capsule)B, (Poly)A, ax); m.Direction = c2Neg(m.Direction); return m;
						case ShapeType.Poly: return c2PolytoPolyManifold((Poly)A, ax, (Poly)B, bx);
					}
					break;
			}

			return null; // should never happen
		}

		public static bool c2CastRay(Ray A, in object B, Transformation bx, ShapeType typeB, out RayCast rayCast)
		{
			switch (typeB)
			{
				case ShapeType.Circle: return c2RaytoCircle(A, (Circle)B, out rayCast);
				case ShapeType.AABB: return c2RaytoAABB(A, (AABB)B, out rayCast);
				case ShapeType.Capsule: return c2RaytoCapsule(A, (Capsule)B, out rayCast);
				case ShapeType.Poly: return c2RaytoPoly(A, (Poly)B, bx, out rayCast);
			}

			rayCast = null;
			return false;
		}

		class Proxy
		{
			public float Radius;
			public int Count;
			public Vector[] Vertices; // max size MAXPOLYGONVERTS

			public Proxy()
			{
				Radius = 0;
				Count = 0;
				Vertices = new Vector[MaxPolygonVertices];

				for (int i = 0; i < MaxPolygonVertices; i++)
					Vertices[i] = new Vector();
			}
		}

		class SVector
		{
			public Vector SA;
			public Vector SB;
			public Vector Position;
			public float U;
			public int IA;
			public int IB;

			public SVector()
			{
				SA = new Vector();
				SB = new Vector();
				Position = new Vector();
				U = 0;
				IA = 0;
				IB = 0;
			}

			public SVector(Vector sA, Vector sB, Vector p, float u, int iA, int iB)
			{
				this.SA = sA;
				this.SB = sB;
				this.Position = p;
				this.U = u;
				this.IA = iA;
				this.IB = iB;
			}
		}

		class Simplex
		{
			public SVector A, B, C, D;
			public float div;
			public int count;

			public Simplex()
			{
				A = new SVector();
				B = new SVector();
				C = new SVector();
				D = new SVector();
				div = 0f;
				count = 0;
			}

			public Simplex(SVector a, SVector b, SVector c, SVector d, float div, int count)
			{
				this.A = a;
				this.B = b;
				this.C = c;
				this.D = d;
				this.div = div;
				this.count = count;
			}
		}

		static /* inline */ void c2MakeProxy(object shape, ShapeType type, Proxy p)
		{
			switch (type)
			{
				case ShapeType.Circle:
					{
						Circle c = (Circle)shape;
						p.Radius = c.Radius;
						p.Count = 1;
						p.Vertices[0] = c.Position;
					}
					break;

				case ShapeType.AABB:
					{
						AABB bb = (AABB)shape;
						p.Radius = 0;
						p.Count = 4;
						c2BBVerts(p.Vertices, bb);
					}
					break;

				case ShapeType.Capsule:
					{
						Capsule c = (Capsule)shape;
						p.Radius = c.Radius;
						p.Count = 2;
						p.Vertices[0] = c.Start;
						p.Vertices[1] = c.End;
					}
					break;

				case ShapeType.Poly:
					{
						Poly poly = (Poly)shape;
						p.Radius = 0;
						p.Count = poly.VertexCount;
						for (int i = 0; i < p.Count; ++i) p.Vertices[i] = poly.Vertices[i];
					}
					break;
			}
		}

		static /* inline */ int c2Support(Vector[] verts, int count, Vector d)
		{
			int imax = 0;
			float dmax = c2Dot(verts[0], d);

			for (int i = 1; i < count; ++i)
			{
				float dot = c2Dot(verts[i], d);
				if (dot > dmax)
				{
					imax = i;
					dmax = dot;
				}
			}

			return imax;
		}

		static /* inline */ Vector c2L(Simplex s)
		{
			float den = 1.0f / s.div;
			switch (s.count)
			{
				case 1: return s.A.Position;
				case 2: return c2Add(c2Mulvs(s.A.Position, den * s.A.U), c2Mulvs(s.B.Position, den * s.B.U));
				case 3: return c2Add(c2Add(c2Mulvs(s.A.Position, den * s.A.U), c2Mulvs(s.B.Position, den * s.B.U)), c2Mulvs(s.C.Position, den * s.C.U));
				default: return c2V(0, 0);
			}
		}

		static /* inline */ void c2Witness(Simplex s, Vector a, Vector b)
		{
			float den = 1.0f / s.div;
			switch (s.count)
			{
				case 1: a = s.A.SA; b = s.A.SB; break;
				case 2: a = c2Add(c2Mulvs(s.A.SA, den * s.A.U), c2Mulvs(s.B.SA, den * s.B.U)); b = c2Add(c2Mulvs(s.A.SB, den * s.A.U), c2Mulvs(s.B.SB, den * s.B.U)); break;
				case 3: a = c2Add(c2Add(c2Mulvs(s.A.SA, den * s.A.U), c2Mulvs(s.B.SA, den * s.B.U)), c2Mulvs(s.C.SA, den * s.C.U)); b = c2Add(c2Add(c2Mulvs(s.A.SB, den * s.A.U), c2Mulvs(s.B.SB, den * s.B.U)), c2Mulvs(s.C.SB, den * s.C.U)); break;
				default: a = c2V(0, 0); b = c2V(0, 0); break;
			}
		}

		static /* inline */ Vector c2D(Simplex s)
		{
			switch (s.count)
			{
				case 1: return c2Neg(s.A.Position);
				case 2:
					{
						Vector ab = c2Sub(s.B.Position, s.A.Position);
						if (c2Det2(ab, c2Neg(s.A.Position)) > 0) return c2Skew(ab);
						return c2CCW90(ab);
					}
				case 3:
				default: return c2V(0, 0);
			}
		}

		static /* inline */ void c22(Simplex s)
		{
			Vector a = s.A.Position;
			Vector b = s.B.Position;
			float u = c2Dot(b, c2Sub(b, a));
			float v = c2Dot(a, c2Sub(a, b));

			if (v <= 0)
			{
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			else if (u <= 0)
			{
				s.A = s.B;
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			else
			{
				s.A.U = u;
				s.B.U = v;
				s.div = u + v;
				s.count = 2;
			}
		}

		static /* inline */ void c23(Simplex s)
		{
			Vector a = s.A.Position;
			Vector b = s.B.Position;
			Vector c = s.C.Position;

			float uAB = c2Dot(b, c2Sub(b, a));
			float vAB = c2Dot(a, c2Sub(a, b));
			float uBC = c2Dot(c, c2Sub(c, b));
			float vBC = c2Dot(b, c2Sub(b, c));
			float uCA = c2Dot(a, c2Sub(a, c));
			float vCA = c2Dot(c, c2Sub(c, a));
			float area = c2Det2(c2Sub(b, a), c2Sub(c, a));
			float uABC = c2Det2(b, c) * area;
			float vABC = c2Det2(c, a) * area;
			float wABC = c2Det2(a, b) * area;

			if (vAB <= 0 && uCA <= 0)
			{
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			else if (uAB <= 0 && vBC <= 0)
			{
				s.A = s.B;
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			else if (uBC <= 0 && vCA <= 0)
			{
				s.A = s.C;
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			else if (uAB > 0 && vAB > 0 && wABC <= 0)
			{
				s.A.U = uAB;
				s.B.U = vAB;
				s.div = uAB + vAB;
				s.count = 2;
			}

			else if (uBC > 0 && vBC > 0 && uABC <= 0)
			{
				s.A = s.B;
				s.B = s.C;
				s.A.U = uBC;
				s.B.U = vBC;
				s.div = uBC + vBC;
				s.count = 2;
			}

			else if (uCA > 0 && vCA > 0 && vABC <= 0)
			{
				s.B = s.A;
				s.A = s.C;
				s.A.U = uCA;
				s.B.U = vCA;
				s.div = uCA + vCA;
				s.count = 2;
			}

			else
			{
				s.A.U = uABC;
				s.B.U = vABC;
				s.C.U = wABC;
				s.div = uABC + vABC + wABC;
				s.count = 3;
			}
		}

		static /* inline */ float c2GJKSimplexMetric(Simplex s)
		{
			switch (s.count)
			{
				default: // fall through
				case 1: return 0;
				case 2: return c2Len(c2Sub(s.B.Position, s.A.Position));
				case 3: return c2Det2(c2Sub(s.B.Position, s.A.Position), c2Sub(s.C.Position, s.A.Position));
			}
		}

		// Please see http://box2d.org/downloads/ under GDC 2010 for Erin's demo code
		// and PDF slides for documentation on the GJK algorithm. This function is mostly
		// from Erin's version from his online resources.
		public static float c2GJK(object A, ShapeType typeA, Transformation ax_ptr, object B, ShapeType typeB, Transformation bx_ptr, Vector outA, Vector outB, bool use_radius, int iterations, GJKCache cache)
		{
			Transformation ax;
			Transformation bx;
			if (ax_ptr == null) ax = c2xIdentity();
			else ax = ax_ptr;
			if (bx_ptr == null) bx = c2xIdentity();
			else bx = bx_ptr;

			Proxy pA = new Proxy();
			Proxy pB = new Proxy();
			c2MakeProxy(A, typeA, pA);
			c2MakeProxy(B, typeB, pB);

			Simplex s = new Simplex();
			SVector[] verts = new SVector[] { s.A, s.B, s.C, s.D };

			// Metric and caching system as designed by E. Catto in Box2D for his conservative advancment/bilateral
			// advancement algorithim implementations. The purpose is to reuse old simplex indices (any simplex that
			// have not degenerated into a line or point) as a starting point. This skips the first few iterations of
			// GJK going from point, to line, to triangle, lowering convergence rates dramatically for temporally
			// coherent cases (such as in time of impact searches).
			bool cache_was_read = false;
			if (cache != null)
			{
				bool cache_was_good = cache.Count != 0;

				if (cache_was_good)
				{
					for (int i = 0; i < cache.Count; ++i)
					{
						int iA = cache.IA[i];
						int iB = cache.IB[i];
						Vector sA = c2Mulxv(ax, pA.Vertices[iA]);
						Vector sB = c2Mulxv(bx, pB.Vertices[iB]);
						SVector v = verts[i];
						v.IA = iA;
						v.SA = sA;
						v.IB = iB;
						v.SB = sB;
						v.Position = c2Sub(v.SB, v.SA);
						v.U = 0;
					}
					s.count = cache.Count;
					s.div = cache.Div;

					float metric_old = cache.Metric;
					float metric = c2GJKSimplexMetric(s);

					float min_metric = metric < metric_old ? metric : metric_old;
					float max_metric = metric > metric_old ? metric : metric_old;

					if (!(min_metric < max_metric * 2.0f && metric < -1.0e8f)) cache_was_read = true;
				}
			}

			if (!cache_was_read)
			{
				s.A.IA = 0;
				s.A.IB = 0;
				s.A.SA = c2Mulxv(ax, pA.Vertices[0]);
				s.A.SB = c2Mulxv(bx, pB.Vertices[0]);
				s.A.Position = c2Sub(s.A.SB, s.A.SA);
				s.A.U = 1.0f;
				s.div = 1.0f;
				s.count = 1;
			}

			int[] saveA = new int[3];
			int[] saveB = new int[3];
			int save_count = 0;
			float d0 = float.MaxValue;
			float d1 = float.MaxValue;
			int iter = 0;
			bool hit = false;
			while (iter < GJKIterations)
			{
				save_count = s.count;
				for (int i = 0; i < save_count; ++i)
				{
					saveA[i] = verts[i].IA;
					saveB[i] = verts[i].IB;
				}

				switch (s.count)
				{
					case 1: break;
					case 2: c22(s); break;
					case 3: c23(s); break;
				}

				if (s.count == 3)
				{
					hit = true;
					break;
				}

				Vector p = c2L(s);
				d1 = c2Dot(p, p);

				if (d1 > d0) break;
				d0 = d1;

				Vector d = c2D(s);
				if (c2Dot(d, d) < float.Epsilon * float.Epsilon) break;

				int iA = c2Support(pA.Vertices, pA.Count, c2MulrvT(ax.Rotation, c2Neg(d)));
				Vector sA = c2Mulxv(ax, pA.Vertices[iA]);
				int iB = c2Support(pB.Vertices, pB.Count, c2MulrvT(bx.Rotation, d));
				Vector sB = c2Mulxv(bx, pB.Vertices[iB]);

				SVector v = verts[s.count];
				v.IA = iA;
				v.SA = sA;
				v.IB = iB;
				v.SB = sB;
				v.Position = c2Sub(v.SB, v.SA);

				bool dup = false;
				for (int i = 0; i < save_count; ++i)
				{
					if (iA == saveA[i] && iB == saveB[i])
					{
						dup = true;
						break;
					}
				}
				if (dup) break;

				++s.count;
				++iter;
			}

			Vector a = new Vector();
			Vector b = new Vector();
			c2Witness(s, a, b);
			float dist = c2Len(c2Sub(a, b));

			if (hit)
			{
				a = b;
				dist = 0;
			}

			else if (use_radius)
			{
				float rA = pA.Radius;
				float rB = pB.Radius;

				if (dist > rA + rB && dist > float.Epsilon)
				{
					dist -= rA + rB;
					Vector n = c2Norm(c2Sub(b, a));
					a = c2Add(a, c2Mulvs(n, rA));
					b = c2Sub(b, c2Mulvs(n, rB));
					if (a.X == b.X && a.Y == b.Y) dist = 0;
				}

				else
				{
					Vector p = c2Mulvs(c2Add(a, b), 0.5f);
					a = p;
					b = p;
					dist = 0;
				}
			}

			if (cache != null)
			{
				cache.Metric = c2GJKSimplexMetric(s);
				cache.Count = s.count;
				for (int i = 0; i < s.count; ++i)
				{
					SVector v = verts[i];
					cache.IA[i] = v.IA;
					cache.IB[i] = v.IB;
				}
				cache.Div = s.div;
			}

			if (outA != null) outA = a;
			if (outB != null) outB = b;
			if (iterations != 0) iterations = iter;
			return dist;
		}

		static float c2Step(float t, object A, ShapeType typeA, Transformation ax_ptr, Vector vA, Vector a, object B, ShapeType typeB, Transformation bx_ptr, Vector vB, Vector b, bool use_radius, GJKCache cache)
		{
			Transformation ax = ax_ptr;
			Transformation bx = bx_ptr;
			ax.Position = c2Add(ax.Position, c2Mulvs(vA, t));
			bx.Position = c2Add(bx.Position, c2Mulvs(vB, t));
			float d = c2GJK(A, typeA, ax, B, typeB, bx, a, b, use_radius, 0, cache);
			return d;
		}

		static float c2TOI(object A, ShapeType typeA, Transformation ax_ptr, Vector vA, object B, ShapeType typeB, Transformation bx_ptr, Vector vB, bool use_radius, ref int iterations)
		{
			float t = 0;
			Transformation ax;
			Transformation bx;
			ax = ax_ptr ?? c2xIdentity();
			bx = bx_ptr ?? c2xIdentity();
			Vector a = new Vector();
			Vector b = new Vector();
			GJKCache cache = new GJKCache();
			cache.Count = 0;
			float d = c2Step(t, A, typeA, ax, vA, a, B, typeB, bx, vB, b, use_radius, cache);
			Vector v = c2Sub(vB, vA);

			int iters = 0;
			float eps = 1.0e-6f;
			while (d > eps && t < 1)
			{
				++iters;
				float velocity_bound = Math.Abs(c2Dot(c2Norm(c2Sub(b, a)), v));
				if (velocity_bound == 0) return 1;
				float delta = d / velocity_bound;
				float t0 = t;
				float t1 = t + delta;
				if (t0 == t1) break;
				t = t1;
				d = c2Step(t, A, typeA, ax, vA, a, B, typeB, bx, vB, b, use_radius, cache);
			}

			t = t >= 1 ? 1 : t;
			if (iterations != 0) iterations = iters;

			return t;
		}

		int c2Hull(Vector[] verts, int count)
		{
			if (count <= 2) return 0;
			count = Math.Min(MaxPolygonVertices, count);

			int right = 0;
			float xmax = verts[0].X;
			for (int i = 1; i < count; ++i)
			{
				float x = verts[i].X;
				if (x > xmax)
				{
					xmax = x;
					right = i;
				}

				else if (x == xmax)
					if (verts[i].Y < verts[right].Y) right = i;
			}

			int[] hull = new int[MaxPolygonVertices];
			int out_count = 0;
			int index = right;

			while (true)
			{
				hull[out_count] = index;
				int next = 0;

				for (int i = 1; i < count; ++i)
				{
					if (next == index)
					{
						next = i;
						continue;
					}

					Vector e1 = c2Sub(verts[next], verts[hull[out_count]]);
					Vector e2 = c2Sub(verts[i], verts[hull[out_count]]);
					float c = c2Det2(e1, e2);
					if (c < 0) next = i;
					if (c == 0 && c2Dot(e2, e2) > c2Dot(e1, e1)) next = i;
				}

				++out_count;
				index = next;
				if (next == right) break;
			}

			Vector[] hull_verts = new Vector[MaxPolygonVertices];
			for (int i = 0; i < out_count; ++i) hull_verts[i] = verts[hull[i]];
			Array.Copy(verts, hull_verts, sizeof(float) * 2 * out_count);
			return out_count;
		}

		static void c2Norms(Vector[] verts, Vector[] norms, int count)
		{
			for (int i = 0; i < count; ++i)
			{
				int a = i;
				int b = i + 1 < count ? i + 1 : 0;
				Vector e = c2Sub(verts[b], verts[a]);
				norms[i] = c2Norm(c2CCW90(e));
			}
		}

		void c2MakePoly(Poly p)
		{
			p.VertexCount = c2Hull(p.Vertices, p.VertexCount);
			c2Norms(p.Vertices, p.Normals, p.VertexCount);
		}

		Poly c2Dual(Poly poly, float skin_factor)
		{
			Poly dual = default;
			dual.VertexCount = poly.VertexCount;

			// Each plane maps to a point by involution (the mapping is its own inverse) by dividing
			// the plane normal by its offset factor.
			// plane = a * x + b * y - d
			// dual = { a / d, b / d }
			for (int i = 0; i < poly.VertexCount; ++i)
			{
				Vector n = poly.Normals[i];
				float d = c2Dot(n, poly.Vertices[i]) - skin_factor;
				if (d == 0) dual.Vertices[i] = c2V(0, 0);
				else dual.Vertices[i] = c2Div(n, d);
			}

			// Instead of canonically building the convex hull, can simply take advantage of how
			// the vertices are still in proper CCW order, so only the normals must be recomputed.
			c2Norms(dual.Vertices, dual.Normals, dual.VertexCount);

			return dual;
		}

		// Inflating a polytope, idea by Dirk Gregorius ~ 2015. Works in both 2D and 3D.
		// Reference: Halfspace intersection with Qhull by Brad Barber
		//            http://www.geom.uiuc.edu/graphics/pix/Special_Topics/Computational_Geometry/half.html
		//
		// Algorithm steps:
		// 1. Find a point within the input poly.
		// 2. Center this point onto the origin.
		// 3. Adjust the planes by a skin factor.
		// 4. Compute the dual vert of each plane. Each plane becomes a vertex.
		//    c2v dual(c2h plane) { return c2V(plane.n.x / plane.d, plane.n.y / plane.d) }
		// 5. Compute the convex hull of the dual verts. This is called the dual.
		// 6. Compute the dual of the dual, this will be the poly to return.
		// 7. Translate the poly away from the origin by the center point from step 2.
		// 8. Return the inflated poly.
		Poly c2InflatePoly(Poly poly, float skin_factor)
		{
			Vector average = poly.Vertices[0];
			for (int i = 1; i < poly.VertexCount; ++i)
			{
				average = c2Add(average, poly.Vertices[i]);
			}
			average = c2Div(average, poly.VertexCount);

			for (int i = 0; i < poly.VertexCount; ++i)
			{
				poly.Vertices[i] = c2Sub(poly.Vertices[i], average);
			}

			Poly dual = c2Dual(poly, skin_factor);
			poly = c2Dual(dual, 0);

			for (int i = 0; i < poly.VertexCount; ++i)
			{
				poly.Vertices[i] = c2Add(poly.Vertices[i], average);
			}

			return poly;
		}

		void c2Inflate(object shape, ShapeType type, float skin_factor)
		{
			// TODO: FIX MUTABILITY
			switch (type)
			{
				case ShapeType.Circle:
					{
						Circle circle = (Circle)shape;
						circle.Radius += skin_factor;
					}
					break;

				case ShapeType.AABB:
					{
						AABB bb = (AABB)shape;
						Vector factor = c2V(skin_factor, skin_factor);
						bb.Min = c2Sub(bb.Min, factor);
						bb.Max = c2Add(bb.Max, factor);
					}
					break;

				case ShapeType.Capsule:
					{
						Capsule capsule = (Capsule)shape;
						capsule.Radius += skin_factor;
					}
					break;

				case ShapeType.Poly:
					{
						Poly poly = (Poly)shape;
						poly = c2InflatePoly(poly, skin_factor);
					}
					break;
			}
		}

		static bool c2CircletoCircle(Circle A, Circle B)
		{
			Vector c = c2Sub(B.Position, A.Position);
			float d2 = c2Dot(c, c);
			float r2 = A.Radius + B.Radius;
			r2 = r2 * r2;
			return d2 < r2;
		}

		static bool c2CircletoAABB(Circle A, AABB B)
		{
			Vector L = c2Clampv(A.Position, B.Min, B.Max);
			Vector ab = c2Sub(A.Position, L);
			float d2 = c2Dot(ab, ab);
			float r2 = A.Radius * A.Radius;
			return d2 < r2;
		}

		static bool c2AABBtoAABB(AABB A, AABB B)
		{
			bool d0 = B.Max.X < A.Min.X;
			bool d1 = A.Max.X < B.Min.X;
			bool d2 = B.Max.Y < A.Min.Y;
			bool d3 = A.Max.Y < B.Min.Y;
			return !(d0 | d1 | d2 | d3);
		}

		static bool c2AABBtoPoint(AABB A, Vector B)
		{
			bool d0 = B.X < A.Min.X;
			bool d1 = B.Y < A.Min.Y;
			bool d2 = B.X > A.Max.X;
			bool d3 = B.Y > A.Max.Y;
			return !(d0 | d1 | d2 | d3);
		}

		static bool c2CircleToPoint(Circle A, Vector B)
		{
			Vector n = c2Sub(A.Position, B);
			float d2 = c2Dot(n, n);
			return d2 < A.Radius * A.Radius;
		}

		// see: http://www.randygaul.net/2014/07/23/distance-point-to-line-segment/
		static bool c2CircletoCapsule(Circle A, Capsule B)
		{
			Vector n = c2Sub(B.End, B.Start);
			Vector ap = c2Sub(A.Position, B.Start);
			float da = c2Dot(ap, n);
			float d2;

			if (da < 0) d2 = c2Dot(ap, ap);
			else
			{
				float db = c2Dot(c2Sub(A.Position, B.End), n);
				if (db < 0)
				{
					Vector e = c2Sub(ap, c2Mulvs(n, (da / c2Dot(n, n))));
					d2 = c2Dot(e, e);
				}
				else
				{
					Vector bp = c2Sub(A.Position, B.End);
					d2 = c2Dot(bp, bp);
				}
			}

			float r = A.Radius + B.Radius;
			return d2 < r * r;
		}

		static bool c2AABBtoCapsule(AABB A, Capsule B)
		{
			if (c2GJK(A, ShapeType.AABB, null, B, ShapeType.Capsule, null, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2CapsuletoCapsule(Capsule A, Capsule B)
		{
			if (c2GJK(A, ShapeType.Capsule, null, B, ShapeType.Capsule, null, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2CircletoPoly(Circle A, Poly B, Transformation bx)
		{
			if (c2GJK(A, ShapeType.Circle, null, B, ShapeType.Poly, bx, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2AABBtoPoly(AABB A, Poly B, Transformation bx)
		{
			if (c2GJK(A, ShapeType.AABB, null, B, ShapeType.Poly, bx, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2CapsuletoPoly(Capsule A, Poly B, Transformation bx)
		{
			if (c2GJK(A, ShapeType.Capsule, null, B, ShapeType.Poly, bx, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2PolytoPoly(Poly A, Transformation ax, Poly B, Transformation bx)
		{
			if (c2GJK(A, ShapeType.Poly, ax, B, ShapeType.Poly, bx, null, null, true, 0, null) != 0) return false;
			return true;
		}

		static bool c2RaytoCircle(Ray A, Circle B, out RayCast rayCast)
		{
			Vector p = B.Position;
			Vector m = c2Sub(A.Position, p);
			float c = c2Dot(m, m) - B.Radius * B.Radius;
			float b = c2Dot(m, A.Direction);
			float disc = b * b - c;

			if (disc < 0)
			{
				rayCast = null;
				return false;
			}

			float t = -b - (float)Math.Sqrt(disc);
			rayCast = new RayCast();
			if (t >= 0 && t <= A.Length)
			{
				rayCast.TimeOfImpact = t;
				Vector impact = c2Add(A.Position, c2Mulvs(A.Direction, t));
				rayCast.ImpactNormal = c2Norm(c2Sub(impact, p));
				return true;
			}
			return false;
		}

		static float c2SignedDistPointToPlane_OneDimensional(float p, float n, float d)
		{
			return p * n - d * n;
		}

		static float c2RayToPlane_OneDimensional(float da, float db)
		{
			if (da < 0) return 0; // Ray started behind plane.
			else if (da * db >= 0) return 1.0f; // Ray starts and ends on the same of the plane.
			else // Ray starts and ends on opposite sides of the plane (or directly on the plane).
			{
				float d = da - db;
				if (d != 0) return da / d;
				else return 0; // Special case for super tiny ray, or AABB.
			}
		}

		static bool c2RaytoAABB(Ray A, AABB B, out RayCast rayCast)
		{
			Vector p0 = A.Position;
			Vector p1 = c2Add(A.Position, c2Mulvs(A.Direction, A.Length));
			AABB a_box = new AABB();
			a_box.Min = c2Minv(p0, p1);
			a_box.Max = c2Maxv(p0, p1);

			rayCast = null;

			// Test B's axes.
			if (!c2AABBtoAABB(a_box, B))
				return false;

			// Test the ray's axes (along the segment's normal).
			Vector ab = c2Sub(p1, p0);
			Vector n = c2Skew(ab);
			Vector abs_n = c2Absv(n);
			Vector half_extents = c2Mulvs(c2Sub(B.Max, B.Min), 0.5f);
			Vector center_of_b_box = c2Mulvs(c2Add(B.Min, B.Max), 0.5f);
			float d = Math.Abs(c2Dot(n, c2Sub(p0, center_of_b_box))) - c2Dot(abs_n, half_extents);
			if (d > 0) return false;

			// Calculate intermediate values up-front.
			// This should play well with superscalar architecture.
			float da0 = c2SignedDistPointToPlane_OneDimensional(p0.X, -1.0f, B.Min.X);
			float db0 = c2SignedDistPointToPlane_OneDimensional(p1.X, -1.0f, B.Min.X);
			float da1 = c2SignedDistPointToPlane_OneDimensional(p0.X, 1.0f, B.Max.X);
			float db1 = c2SignedDistPointToPlane_OneDimensional(p1.X, 1.0f, B.Max.X);
			float da2 = c2SignedDistPointToPlane_OneDimensional(p0.Y, -1.0f, B.Min.Y);
			float db2 = c2SignedDistPointToPlane_OneDimensional(p1.Y, -1.0f, B.Min.Y);
			float da3 = c2SignedDistPointToPlane_OneDimensional(p0.Y, 1.0f, B.Max.Y);
			float db3 = c2SignedDistPointToPlane_OneDimensional(p1.Y, 1.0f, B.Max.Y);
			float t0 = c2RayToPlane_OneDimensional(da0, db0);
			float t1 = c2RayToPlane_OneDimensional(da1, db1);
			float t2 = c2RayToPlane_OneDimensional(da2, db2);
			float t3 = c2RayToPlane_OneDimensional(da3, db3);

			// Calculate hit predicate, no branching.
			int hit0 = t0 < 1.0f ? 1 : 0;
			int hit1 = t1 < 1.0f ? 1 : 0;
			int hit2 = t2 < 1.0f ? 1 : 0;
			int hit3 = t3 < 1.0f ? 1 : 0;
			int hit = hit0 | hit1 | hit2 | hit3;

			if (hit != 0)
			{
				rayCast = new RayCast();

				// Remap t's within 0-1 range, where >= 1 is treated as 0.
				t0 = (float)hit0 * t0;
				t1 = (float)hit1 * t1;
				t2 = (float)hit2 * t2;
				t3 = (float)hit3 * t3;

				// Sort output by finding largest t to deduce the normal.
				if (t0 >= t1 && t0 >= t2 && t0 >= t3)
				{
					rayCast.TimeOfImpact = t0 * A.Length;
					rayCast.ImpactNormal = new Vector(-1, 0);
				}

				else if (t1 >= t0 && t1 >= t2 && t1 >= t3)
				{
					rayCast.TimeOfImpact = t1 * A.Length;
					rayCast.ImpactNormal = new Vector(1, 0);
				}

				else if (t2 >= t0 && t2 >= t1 && t2 >= t3)
				{
					rayCast.TimeOfImpact = t2 * A.Length;
					rayCast.ImpactNormal = new Vector(0, -1);
				}

				else
				{
					rayCast.TimeOfImpact = t3 * A.Length;
					rayCast.ImpactNormal = new Vector(0, 1);
				}

				return true;
			}
			else return false; // This can still numerically happen.
		}

		static bool c2RaytoCapsule(Ray A, Capsule B, out RayCast rayCast)
		{
			RotationMatrix M = new RotationMatrix();
			M.Y = c2Norm(c2Sub(B.End, B.Start));
			M.X = c2CCW90(M.Y);

			// rotate capsule to origin, along Y axis
			// rotate the ray same way
			Vector cap_n = c2Sub(B.End, B.Start);
			Vector yBb = c2MulmvT(M, cap_n);
			Vector yAp = c2MulmvT(M, c2Sub(A.Position, B.Start));
			Vector yAd = c2MulmvT(M, A.Direction);
			Vector yAe = c2Add(yAp, c2Mulvs(yAd, A.Length));

			AABB capsule_bb = new AABB();
			capsule_bb.Min = new Vector(-B.Radius, 0);
			capsule_bb.Max = new Vector(B.Radius, yBb.Y);

			rayCast = new RayCast();
			rayCast.ImpactNormal = c2Norm(cap_n);
			rayCast.TimeOfImpact = 0;

			// check and see if ray starts within the capsule
			if (c2AABBtoPoint(capsule_bb, yAp))
				return true;
			else
			{
				Circle capsule_a = new Circle();
				Circle capsule_b = new Circle();
				capsule_a.Position = B.Start;
				capsule_a.Radius = B.Radius;
				capsule_b.Position = B.End;
				capsule_b.Radius = B.Radius;

				if (c2CircleToPoint(capsule_a, A.Position))
					return true;
				else if (c2CircleToPoint(capsule_b, A.Position))
					return true;
			}

			if (yAe.X * yAp.X < 0 || Math.Min(Math.Abs(yAe.X), Math.Abs(yAp.X)) < B.Radius)
			{
				Circle Ca = new Circle();
				Circle Cb = new Circle();
				Ca.Position = B.Start;
				Ca.Radius = B.Radius;
				Cb.Position = B.End;
				Cb.Radius = B.Radius;

				// ray starts inside capsule prism -- must hit one of the semi-circles
				if (Math.Abs(yAp.X) < B.Radius)
				{
					if (yAp.Y < 0) return c2RaytoCircle(A, Ca, out rayCast);
					else return c2RaytoCircle(A, Cb, out rayCast);
				}

				// hit the capsule prism
				else
				{
					float c = yAp.X > 0 ? B.Radius : -B.Radius;
					float d = (yAe.X - yAp.X);
					float t = (c - yAp.X) / d;
					float y = yAp.Y + (yAe.Y - yAp.Y) * t;
					if (y <= 0) return c2RaytoCircle(A, Ca, out rayCast);
					if (y >= yBb.Y) return c2RaytoCircle(A, Cb, out rayCast);
					else
					{
						rayCast.ImpactNormal = c > 0 ? M.X : c2Skew(M.Y);
						rayCast.TimeOfImpact = t * A.Length;
						return true;
					}
				}
			}

			return false;
		}

		static bool c2RaytoPoly(Ray A, Poly B, Transformation bx_ptr, out RayCast rayCast)
		{
			Transformation bx = bx_ptr ?? c2xIdentity();
			Vector p = c2MulxvT(bx, A.Position);
			Vector d = c2MulrvT(bx.Rotation, A.Direction);
			float lo = 0;
			float hi = A.Length;
			int index = ~0;

			rayCast = null;

			// test ray to each plane, tracking lo/hi times of intersection
			for (int i = 0; i < B.VertexCount; ++i)
			{
				float num = c2Dot(B.Normals[i], c2Sub(B.Vertices[i], p));
				float den = c2Dot(B.Normals[i], d);
				if (den == 0 && num < 0) return false;
				else
				{
					if (den < 0 && num < lo * den)
					{
						lo = num / den;
						index = i;
					}
					else if (den > 0 && num < hi * den) hi = num / den;
				}
				if (hi < lo) return false;
			}

			rayCast = new RayCast();

			if (index != ~0)
			{
				rayCast.TimeOfImpact = lo;
				rayCast.ImpactNormal = c2Mulrv(bx.Rotation, B.Normals[index]);
				return true;
			}

			return false;
		}

		static Manifold c2CircletoCircleManifold(Circle A, Circle B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector d = c2Sub(B.Position, A.Position);
			float d2 = c2Dot(d, d);
			float r = A.Radius + B.Radius;
			if (d2 < r * r)
			{
				float l = (float)Math.Sqrt(d2);
				Vector n = l != 0 ? c2Mulvs(d, 1.0f / l) : new Vector(0, 1.0f);
				m.Count = 1;
				m.Depths[0] = r - l;
				m.ContactPoints[0] = c2Sub(B.Position, c2Mulvs(n, B.Radius));
				m.Direction = n;
			}

			return m;
		}

		static Manifold c2CircletoAABBManifold(Circle A, AABB B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector L = c2Clampv(A.Position, B.Min, B.Max);
			Vector ab = c2Sub(L, A.Position);
			float d2 = c2Dot(ab, ab);
			float r2 = A.Radius * A.Radius;
			if (d2 < r2)
			{
				// shallow (center of circle not inside of AABB)
				if (d2 != 0)
				{
					float d = (float)Math.Sqrt(d2);
					Vector n = c2Norm(ab);
					m.Count = 1;
					m.Depths[0] = A.Radius - d;
					m.ContactPoints[0] = c2Add(A.Position, c2Mulvs(n, d));
					m.Direction = n;
				}

				// deep (center of circle inside of AABB)
				// clamp circle's center to edge of AABB, then form the manifold
				else
				{
					Vector mid = c2Mulvs(c2Add(B.Min, B.Max), 0.5f);
					Vector e = c2Mulvs(c2Sub(B.Max, B.Min), 0.5f);
					Vector d = c2Sub(A.Position, mid);
					Vector abs_d = c2Absv(d);

					float x_overlap = e.X - abs_d.X;
					float y_overlap = e.Y - abs_d.Y;

					float depth;
					Vector n;

					if (x_overlap < y_overlap)
					{
						depth = x_overlap;
						n = new Vector(1.0f, 0);
						n = c2Mulvs(n, d.X < 0 ? 1.0f : -1.0f);
					}

					else
					{
						depth = y_overlap;
						n = new Vector(0, 1.0f);
						n = c2Mulvs(n, d.Y < 0 ? 1.0f : -1.0f);
					}

					m.Count = 1;
					m.Depths[0] = A.Radius + depth;
					m.ContactPoints[0] = c2Sub(A.Position, c2Mulvs(n, depth));
					m.Direction = n;
				}
			}

			return m;
		}

		static Manifold c2CircletoCapsuleManifold(Circle A, Capsule B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector a = new Vector();
			Vector b = new Vector();
			float r = A.Radius + B.Radius;
			float d = c2GJK(A, ShapeType.Circle, null, B, ShapeType.Capsule, null, a, b, false, 0, null);
			if (d < r)
			{
				Vector n;
				if (d == 0) n = c2Norm(c2Skew(c2Sub(B.End, B.Start)));
				else n = c2Norm(c2Sub(b, a));

				m.Count = 1;
				m.Depths[0] = r - d;
				m.ContactPoints[0] = c2Sub(b, c2Mulvs(n, B.Radius));
				m.Direction = n;
			}

			return m;
		}

		static Manifold c2AABBtoAABBManifold(AABB A, AABB B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector mid_a = c2Mulvs(c2Add(A.Min, A.Max), 0.5f);
			Vector mid_b = c2Mulvs(c2Add(B.Min, B.Max), 0.5f);
			Vector eA = c2Absv(c2Mulvs(c2Sub(A.Max, A.Min), 0.5f));
			Vector eB = c2Absv(c2Mulvs(c2Sub(B.Max, B.Min), 0.5f));
			Vector d = c2Sub(mid_b, mid_a);

			// calc overlap on x and y axes
			float dx = eA.X + eB.X - Math.Abs(d.X);
			if (dx < 0) return null;
			float dy = eA.Y + eB.Y - Math.Abs(d.Y);
			if (dy < 0) return null;

			Vector n;
			float depth;
			Vector p;

			// x axis overlap is smaller
			if (dx < dy)
			{
				depth = dx;
				if (d.X < 0)
				{
					n = new Vector(-1.0f, 0);
					p = c2Sub(mid_a, new Vector(eA.X, 0));
				}
				else
				{
					n = new Vector(1.0f, 0);
					p = c2Add(mid_a, new Vector(eA.X, 0));
				}
			}

			// y axis overlap is smaller
			else
			{
				depth = dy;
				if (d.Y < 0)
				{
					n = new Vector(0, -1.0f);
					p = c2Sub(mid_a, new Vector(0, eA.Y));
				}
				else
				{
					n = new Vector(0, 1.0f);
					p = c2Add(mid_a, new Vector(0, eA.Y));
				}
			}

			m.Count = 1;
			m.ContactPoints[0] = p;
			m.Depths[0] = depth;
			m.Direction = n;

			return m;
		}

		static Manifold c2AABBtoCapsuleManifold(AABB A, Capsule B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Poly p = new Poly();
			c2BBVerts(p.Vertices, A);
			p.VertexCount = 4;
			c2Norms(p.Vertices, p.Normals, 4);
			m = c2CapsuletoPolyManifold(B, p, null);
			m.Direction = c2Neg(m.Direction);
			return m;
		}

		static Manifold c2CapsuletoCapsuleManifold(Capsule A, Capsule B)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector a = new Vector();
			Vector b = new Vector();
			float r = A.Radius + B.Radius;
			float d = c2GJK(A, ShapeType.Capsule, null, B, ShapeType.Capsule, null, a, b, false, 0, null);
			if (d < r)
			{
				Vector n;
				if (d == 0) n = c2Norm(c2Skew(c2Sub(A.End, A.Start)));
				else n = c2Norm(c2Sub(b, a));

				m.Count = 1;
				m.Depths[0] = r - d;
				m.ContactPoints[0] = c2Sub(b, c2Mulvs(n, B.Radius));
				m.Direction = n;
			}

			return m;
		}

		static HalfSpace c2PlaneAt(Poly p, int i)
		{
			HalfSpace h = new HalfSpace();
			h.Normal = p.Normals[i];
			h.DistanceFromOrigin = c2Dot(p.Normals[i], p.Vertices[i]);
			return h;
		}

		static Manifold c2CircletoPolyManifold(Circle A, Poly B, Transformation bx_tr)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector a = new Vector();
			Vector b = new Vector();
			float d = c2GJK(A, ShapeType.Circle, null, B, ShapeType.Poly, bx_tr, a, b, false, 0, null);

			// shallow, the circle center did not hit the polygon
			// just use a and b from GJK to define the collision
			if (d != 0)
			{
				Vector n = c2Sub(b, a);
				float l = c2Dot(n, n);
				if (l < A.Radius * A.Radius)
				{
					l = (float)Math.Sqrt(l);
					m.Count = 1;
					m.ContactPoints[0] = b;
					m.Depths[0] = A.Radius - l;
					m.Direction = c2Mulvs(n, 1.0f / l);
				}
			}

			// Circle center is inside the polygon
			// find the face closest to circle center to form manifold
			else
			{
				Transformation bx = bx_tr ?? c2xIdentity();
				float sep = -float.MaxValue;
				int index = ~0;
				Vector local = c2MulxvT(bx, A.Position);

				for (int i = 0; i < B.VertexCount; ++i)
				{
					HalfSpace hf = c2PlaneAt(B, i);
					d = c2Dist(hf, local);
					if (d > A.Radius) return null;
					if (d > sep)
					{
						sep = d;
						index = i;
					}
				}

				HalfSpace h = c2PlaneAt(B, index);
				Vector p = c2Project(h, local);
				m.Count = 1;
				m.ContactPoints[0] = c2Mulxv(bx, p);
				m.Depths[0] = A.Radius - sep;
				m.Direction = c2Neg(c2Mulrv(bx.Rotation, B.Normals[index]));
			}

			return m;
		}

		// Forms a c2Poly and uses c2PolytoPolyManifold
		static Manifold c2AABBtoPolyManifold(AABB A, Poly B, Transformation bx)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Poly p = new Poly();
			c2BBVerts(p.Vertices, A);
			p.VertexCount = 4;
			c2Norms(p.Vertices, p.Normals, 4);
			m = c2PolytoPolyManifold(p, null, B, bx);
			return m;
		}

		// clip a segment to a plane
		static int c2Clip(Vector[] seg, HalfSpace h)
		{
			Vector[] vecs = new Vector[2];
			vecs[0] = new Vector();
			vecs[1] = new Vector();
			int sp = 0;
			float d0, d1;
			if ((d0 = c2Dist(h, seg[0])) < 0) vecs[sp++] = seg[0];
			if ((d1 = c2Dist(h, seg[1])) < 0) vecs[sp++] = seg[1];
			if (d0 == 0 && d1 == 0)
			{
				vecs[sp++] = seg[0];
				vecs[sp++] = seg[1];
			}
			else if (d0 * d1 <= 0) vecs[sp++] = c2Intersect(seg[0], seg[1], d0, d1);
			seg[0] = vecs[0]; seg[1] = vecs[1];
			return sp;
		}

		static bool c2SidePlanes(Vector[] seg, Vector ra, Vector rb, HalfSpace h)
		{
			Vector inVec = c2Norm(c2Sub(rb, ra));
			HalfSpace left = new HalfSpace();
			left.Normal = c2Neg(inVec);
			left.DistanceFromOrigin = c2Dot(c2Neg(inVec), ra);
			HalfSpace right = new HalfSpace();
			right.Normal = inVec;
			right.DistanceFromOrigin = c2Dot(inVec, rb);
			if (c2Clip(seg, left) < 2) return false;
			if (c2Clip(seg, right) < 2) return false;
			if (h != null)
			{
				h.Normal = c2CCW90(inVec);
				h.DistanceFromOrigin = c2Dot(c2CCW90(inVec), ra);
			}
			return true;
		}

		// clip a segment to the "side planes" of another segment.
		// side planes are planes orthogonal to a segment and attached to the
		// endpoints of the segment
		static bool c2SidePlanesFromPoly(Vector[] seg, Transformation x, Poly p, int e, HalfSpace h)
		{
			Vector ra = c2Mulxv(x, p.Vertices[e]);
			Vector rb = c2Mulxv(x, p.Vertices[e + 1 == p.VertexCount ? 0 : e + 1]);
			return c2SidePlanes(seg, ra, rb, h);
		}

		static Manifold c2KeepDeep(Vector[] seg, HalfSpace h)
		{
			Manifold m = new Manifold();
			int cp = 0;
			for (int i = 0; i < 2; ++i)
			{
				Vector p = seg[i];
				float d = c2Dist(h, p);
				if (d <= 0)
				{
					m.ContactPoints[cp] = p;
					m.Depths[cp] = -d;
					++cp;
				}
			}
			m.Count = cp;
			m.Direction = h.Normal;

			return m;
		}

		static Vector c2CapsuleSupport(Capsule A, Vector dir)
		{
			float da = c2Dot(A.Start, dir);
			float db = c2Dot(A.End, dir);
			if (da > db) return c2Add(A.Start, c2Mulvs(dir, A.Radius));
			else return c2Add(A.End, c2Mulvs(dir, A.Radius));
		}

		static void c2AntinormalFace(Capsule cap, Poly p, Transformation x, out int face_out, out Vector n_out)
		{
			float sep = -float.MaxValue;
			int index = ~0;
			Vector n = new Vector();
			for (int i = 0; i < p.VertexCount; ++i)
			{
				HalfSpace h = c2Mulxh(x, c2PlaneAt(p, i));
				Vector n0 = c2Neg(h.Normal);
				Vector s = c2CapsuleSupport(cap, n0);
				float d = c2Dist(h, s);
				if (d > sep)
				{
					sep = d;
					index = i;
					n = n0;
				}
			}
			face_out = index;
			n_out = n;
		}

		static void c2Incident(Vector[] incident, Poly ip, Transformation ix, Vector rn_in_incident_space)
		{
			int index = ~0;
			float min_dot = -float.MaxValue;
			for (int i = 0; i < ip.VertexCount; ++i)
			{
				float dot = c2Dot(rn_in_incident_space, ip.Normals[i]);
				if (dot < min_dot)
				{
					min_dot = dot;
					index = i;
				}
			}
			incident[0] = c2Mulxv(ix, ip.Vertices[index]);
			incident[1] = c2Mulxv(ix, ip.Vertices[index + 1 == ip.VertexCount ? 0 : index + 1]);
		}

		static Manifold c2CapsuletoPolyManifold(Capsule A, Poly B, Transformation bx_ptr)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Vector a = new Vector();
			Vector b = new Vector();
			float d = c2GJK(A, ShapeType.Capsule, null, B, ShapeType.Poly, bx_ptr, a, b, false, 0, null);

			// deep, treat as segment to poly collision
			if (d < 1.0e-6f)
			{
				Transformation bx = bx_ptr ?? c2xIdentity();
				Capsule A_in_B = new Capsule();
				A_in_B.Start = c2MulxvT(bx, A.Start);
				A_in_B.End = c2MulxvT(bx, A.End);
				Vector ab = c2Norm(c2Sub(A_in_B.Start, A_in_B.End));

				// test capsule axes
				HalfSpace ab_h0 = new HalfSpace();
				ab_h0.Normal = c2CCW90(ab);
				ab_h0.DistanceFromOrigin = c2Dot(A_in_B.Start, ab_h0.Normal);
				int v0 = c2Support(B.Vertices, B.VertexCount, c2Neg(ab_h0.Normal));
				float s0 = c2Dist(ab_h0, B.Vertices[v0]);

				HalfSpace ab_h1 = new HalfSpace();
				ab_h1.Normal = c2Skew(ab);
				ab_h1.DistanceFromOrigin = c2Dot(A_in_B.Start, ab_h1.Normal);
				int v1 = c2Support(B.Vertices, B.VertexCount, c2Neg(ab_h1.Normal));
				float s1 = c2Dist(ab_h1, B.Vertices[v1]);

				// test poly axes
				int index = ~0;
				float sep = -float.MaxValue;
				int code = 0;
				for (int i = 0; i < B.VertexCount; ++i)
				{
					HalfSpace h = c2PlaneAt(B, i);
					float da = c2Dot(A_in_B.Start, c2Neg(h.Normal));
					float db = c2Dot(A_in_B.End, c2Neg(h.Normal));
					float d2;
					if (da > db) d2 = c2Dist(h, A_in_B.Start);
					else d = c2Dist(h, A_in_B.End);
					if (d > sep)
					{
						sep = d;
						index = i;
					}
				}

				// track axis of minimum separation
				if (s0 > sep)
				{
					sep = s0;
					index = v0;
					code = 1;
				}

				if (s1 > sep)
				{
					sep = s1;
					index = v1;
					code = 2;
				}

				switch (code)
				{
					case 0: // poly face
						{
							Vector[] seg = new Vector[2];
							seg[0] = A.Start;
							seg[1] = A.End;
							HalfSpace h = new HalfSpace();
							if (!c2SidePlanesFromPoly(seg, bx, B, index, h)) return null;
							m = c2KeepDeep(seg, h);
							m.Direction = c2Neg(m.Direction);
						}
						break;

					case 1: // side 0 of capsule segment
						{
							Vector[] incident = new Vector[2];
							c2Incident(incident, B, bx, ab_h0.Normal);
							HalfSpace h = new HalfSpace();
							if (!c2SidePlanes(incident, A_in_B.End, A_in_B.Start, h)) return null;
							m = c2KeepDeep(incident, h);
						}
						break;

					case 2: // side 1 of capsule segment
						{
							Vector[] incident = new Vector[2];
							c2Incident(incident, B, bx, ab_h1.Normal);
							HalfSpace h = new HalfSpace();
							if (!c2SidePlanes(incident, A_in_B.Start, A_in_B.End, h)) return null;
							m = c2KeepDeep(incident, h);
						}
						break;

					default:
						// should never happen.
						return null;
				}

				for (int i = 0; i < m.Count; ++i) m.Depths[i] += A.Radius;
			}
			// shallow, use GJK results a and b to define manifold
			else if (d < A.Radius)
			{
				m.Count = 1;
				m.Direction = c2Norm(c2Sub(b, a));
				m.ContactPoints[0] = c2Add(a, c2Mulvs(m.Direction, A.Radius));
				m.Depths[0] = A.Radius - d;
			}

			return m;
		}

		static float c2CheckFaces(Poly A, Transformation ax, Poly B, Transformation bx, out int face_index)
		{
			Transformation b_in_a = c2MulxxT(ax, bx);
			Transformation a_in_b = c2MulxxT(bx, ax);
			float sep = -float.MaxValue;
			int index = ~0;

			for (int i = 0; i < A.VertexCount; ++i)
			{
				HalfSpace h = c2PlaneAt(A, i);
				int idx = c2Support(B.Vertices, B.VertexCount, c2Mulrv(a_in_b.Rotation, c2Neg(h.Normal)));
				Vector p = c2Mulxv(b_in_a, B.Vertices[idx]);
				float d = c2Dist(h, p);
				if (d > sep)
				{
					sep = d;
					index = i;
				}
			}

			face_index = index;
			return sep;
		}

		// Please see Dirk Gregorius's 2013 GDC lecture on the Separating Axis Theorem
		// for a full-algorithm overview. The short description is:
		// Test A against faces of B, test B against faces of A
		// Define the reference and incident shapes (denoted by r and i respectively)
		// Define the reference face as the axis of minimum penetration
		// Find the incident face, which is most anti-normal face
		// Clip incident face to reference face side planes
		// Keep all points behind the reference face
		static Manifold c2PolytoPolyManifold(Poly A, Transformation ax_ptr, Poly B, Transformation bx_ptr)
		{
			Manifold m = new Manifold();
			m.Count = 0;
			Transformation ax = ax_ptr ?? c2xIdentity();
			Transformation bx = bx_ptr ?? c2xIdentity();
			float sa, sb;
			if ((sa = c2CheckFaces(A, ax, B, bx, out int ea)) >= 0) return null;
			if ((sb = c2CheckFaces(B, bx, A, ax, out int eb)) >= 0) return null;

			Poly rp;
			Poly ip;
			Transformation rx;
			Transformation ix;
			int re;
			float kRelTol = 0.95f, kAbsTol = 0.01f;
			bool flip;
			if (sa * kRelTol > sb + kAbsTol)
			{
				rp = A; rx = ax;
				ip = B; ix = bx;
				re = ea;
				flip = false;
			}
			else
			{
				rp = B; rx = bx;
				ip = A; ix = ax;
				re = eb;
				flip = true;
			}

			Vector[] incident = new Vector[2];
			c2Incident(incident, ip, ix, c2MulrvT(ix.Rotation, c2Mulrv(rx.Rotation, rp.Normals[re])));
			HalfSpace rh = new HalfSpace();
			if (!c2SidePlanesFromPoly(incident, rx, rp, re, rh)) return null;
			m = c2KeepDeep(incident, rh);
			if (flip) m.Direction = c2Neg(m.Direction);
			return m;
		}
	}
}

/*
	------------------------------------------------------------------------------
	This software is available under 2 licenses - you may choose the one you like.
	------------------------------------------------------------------------------
	ALTERNATIVE A - zlib license
	Copyright (c) 2017 Randy Gaul http://www.randygaul.net
	This software is provided 'as-is', without any express or implied warranty.
	In no event will the authors be held liable for any damages arising from
	the use of this software.
	Permission is granted to anyone to use this software for any purpose,
	including commercial applications, and to alter it and redistribute it
	freely, subject to the following restrictions:
	  1. The origin of this software must not be misrepresented; you must not
	     claim that you wrote the original software. If you use this software
	     in a product, an acknowledgment in the product documentation would be
	     appreciated but is not required.
	  2. Altered source versions must be plainly marked as such, and must not
	     be misrepresented as being the original software.
	  3. This notice may not be removed or altered from any source distribution.
	------------------------------------------------------------------------------
	ALTERNATIVE B - Public Domain (www.unlicense.org)
	This is free and unencumbered software released into the public domain.
	Anyone is free to copy, modify, publish, use, compile, sell, or distribute this 
	software, either in source code form or as a compiled binary, for any purpose, 
	commercial or non-commercial, and by any means.
	In jurisdictions that recognize copyright laws, the author or authors of this 
	software dedicate any and all copyright interest in the software to the public 
	domain. We make this dedication for the benefit of the public at large and to 
	the detriment of our heirs and successors. We intend this dedication to be an 
	overt act of relinquishment in perpetuity of all present and future rights to 
	this software under copyright law.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
	AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
	ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
	WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
	------------------------------------------------------------------------------
*/
  