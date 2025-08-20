using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ ��� ���̺� Ŭ����
/// �� ����(1~10)�� ���� ��� Ȯ���� �����ϰ� ����մϴ�.
/// </summary>
[System.Serializable]
public class DropTable
{
    public string name; // ��� ���̺� �̸� (���� �̸�)
    public float[] weights = new float[10]; // �� ����(1~10)�� Ȯ�� ����ġ

    public DropTable(string name)
    {
        this.name = name;
    }

    /// <summary>
    /// ���Ժ��� ��� Ȯ�� ���� ����
    /// �÷��̾� ������ ǥ�������� ������� ����ġ ���̺��� ���� (�����ۺ� ��� Ȯ��(weight) ���)
    /// </summary>
    /// <param name="characterLevel">���� ĳ���� ���� (1~100)</param>
    /// <param name="stdDev">ǥ������: �������� �л��� ŭ</param>
    public void GenerateDistribution(int characterLevel, float stdDev)
    {
        // ��� ���� ���: ĳ���� ������ �������� ��յ� ��� (�ִ� 10)
        // EX> ���� 50�̸� ���(mean) = 6
        //     ǥ������ 1.5�̸� ������ ���� = 4~8
        float mean = Mathf.Lerp(2f, 10f, characterLevel / 100f); // ĳ���� ������ ���� ��� ������ ���� ���

        float total = 0f;

        // ���� 1~10�� ���� ���Ժ��� Ȯ�� ���
        for (int i = 0; i < 10; i++)
        {
            weights[i] = Gaussian(i + 1, mean, stdDev); // i + 1 == ������ ����
            total += weights[i]; // ��ü �ջ� (����ȭ��)
        }

        // ��ü ������ ����ȭ => ������ 1�� �ǵ��� Ȯ��ȭ
        for (int i = 0; i < 10; i++)
        {
            weights[i] /= total;
        }
    }

    /// <summary>
    /// ���� ����ġ ������� ������ ���� ����
    /// Ȯ�� ���� ��� ���
    /// </summary>
    /// <returns>���õ� ������ ���� (1~10)</returns>
    public int GetWeightedItemLevel()
    {
        float rand = UnityEngine.Random.value; // 0~1 ���� ����
        float cumulative = 0f; // ���� Ȯ��

        for (int i = 0; i < 10; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative)
                return i + 1; // ���� ������ �ε��� + 1
        }

        return 1; // ���� ������ Fallback
    }

    /// <summary>
    /// ���Ժ���(���콺) Ȯ�� �е� �Լ�
    /// </summary>
    /// <param name="x">���� ���� (������ ����)</param>
    /// <param name="mean">��� (�߽ɰ�), �������� ���� ���� ������ ����</param>
    /// <param name="stdDev">ǥ������, Ŭ���� �پ��� ���� ����</param>
    /// <returns>�ش� ������ Ȯ�� ��, X���� ���� Ȯ�� �� ��</returns>
    float Gaussian(float x, float mean, float stdDev) // f(x) = (1 / ��(2����)) * exp(-((x - ��)�� / (2���)))
    {
        float a = 1.0f / (stdDev * Mathf.Sqrt(2.0f * Mathf.PI)); // ����ȭ ��� // a = (1 / ��(2����))
        float b = Mathf.Exp(-Mathf.Pow(x - mean, 2) / (2.0f * stdDev * stdDev)); // �����Լ� // b = exp(-((x - ��)�� / (2���)))
        return a * b; // ���콺 �Լ� ��� // f(x)
    }
}
