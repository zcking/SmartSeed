using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Defines the model for optimization
public class Plant
{

	public static long plantCount = 0;

	public int leafCount = 0;
	public float bushHeight = 0.0f;
	public float bushRadius = 0.0f;
	public int berryCount = 0;
	public int branches = 0;

	public const uint LIFE_CYCLE_COUNT = 100;

	private Solution solution;

	public Plant() {
	}

	public Plant(Plant other)
	{
		solution = other.solution;
		leafCount = other.leafCount;
		bushHeight = other.bushHeight;
		bushRadius = other.bushRadius;
		berryCount = other.berryCount;
		branches = other.branches;
	}

	public void Init(Solution sol)
	{
		solution = sol;
		plantCount++;

		bushHeight = 0.001f;
		bushRadius = 0.001f;
	}

	public void RunCycle()
	{
		// Define what happens to the plant model's attributes based on the given solution
		// Note - This changes based on the plant; here I am using a blueberry bush
		float area = 4.0f * 3.14159f * bushRadius * bushRadius;
		float oldAreaRatio = area % 0.75f;

		if (solution.soilMoisture >= 0.7f && solution.soilMoisture <= 0.8f) {
			if (solution.sunlight >= 0.65f && solution.sunlight <= 0.7f) {
				if (solution.humidity >= 0.8f && solution.humidity <= 0.85f) {
					if (solution.temperature >= 60.0f && solution.temperature <= 70.0f) {
						bushHeight += 0.1f;
						bushRadius += 0.1f;
					}
				}
			}
		}

		if (solution.soilMoisture >= 0.7f && solution.soilMoisture <= 0.73f) {
			if (solution.sunlight >= 0.68f && solution.sunlight <= 0.7f) {
				if (solution.humidity >= 0.8f && solution.humidity <= 0.83f) {
					if (solution.temperature >= 68.0f && solution.temperature <= 70.0f) {
						bushHeight += 0.5f;
						bushRadius += 0.5f;
					}
				}
			}
		}

		if (solution.soilMoisture >= 0.71f && solution.soilMoisture <= 0.72f) {
			if (solution.sunlight >= 0.68f && solution.sunlight <= 0.69f) {
				if (solution.humidity >= 0.8f && solution.humidity <= 0.81f) {
					if (solution.temperature >= 68.0f && solution.temperature <= 69.0f) {
						bushHeight += 1.0f;
						bushRadius += 1.0f;
					}
				}
			}
		}
			


		area = 4.0f * 3.14159f * bushRadius * bushRadius;
		if (area >= oldAreaRatio) {
			branches++;
		}

		leafCount++;
		berryCount++;

		if (branches == 0)
			berryCount = 0;

		if (bushHeight < 0.07f)
			leafCount = 0;
	}

	private void revertState(Plant savedState) {
		this.bushHeight = savedState.bushHeight;
		this.bushRadius = savedState.bushRadius;
		this.branches = savedState.branches;
		this.berryCount = savedState.berryCount;
		this.leafCount = savedState.leafCount;
	}

	public float CalculateFitness()
	{
		Plant savedState = new Plant (this);

		// Define the fitness equation based on the plant's attributes
		for (int i = 0; i < LIFE_CYCLE_COUNT; i++)
			RunCycle ();

		float result;
		if (bushRadius == 0.0f)
			result = 0.0f;
		else if (branches == 0.0f || leafCount == 0)
			result = 0.0001f;
		else
			result = (berryCount / LIFE_CYCLE_COUNT) * (leafCount / LIFE_CYCLE_COUNT) * ((4.0f * 3.14159f * bushRadius * bushRadius) / 0.0012f) * (branches / LIFE_CYCLE_COUNT);
//			result = ((berryCount / leafCount) / branches) / (4.0f * 3.14159f * bushRadius * bushRadius);

		revertState (savedState);

		return result;
	}

	public float getHeight() {
		Plant savedState = new Plant (this);

		for (int i = 0; i < LIFE_CYCLE_COUNT; i++)
			RunCycle ();

		float result = this.bushHeight;
		revertState (savedState);
		return result;
	}

	public float getRadius() {
		Plant savedState = new Plant (this);

		for (int i = 0; i < LIFE_CYCLE_COUNT; i++) {
			RunCycle ();
		}

		if (this.bushRadius == 0.001f) {
			Debug.LogWarning ("Bush radius didn't change (still 0.001); BH=" + bushHeight + ", BR=" + branches + ", BC=" + berryCount + ", LC=" + leafCount);
			Debug.LogWarning ("\t" + solution.toString ());
		}

		float result = this.bushRadius;
		revertState (savedState);
		return result;
	}

}

