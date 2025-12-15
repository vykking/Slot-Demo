using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    #region SpinSetting
    [Header("Spin Settings")]
    public float speed = 5f;
    public float slowSpeed = 0.4f;
    public float decel = 3f;
    private float currentSpeed;
    public float symbolHeight = 1.5f;
    #endregion

    #region 状态
    [Header("State")]
    public bool isSpinning = false;
    private bool isStopping = false;
    private bool isSnapping = false;
    public bool IsSnapped => !isSpinning && !isStopping && !isSnapping;
    #endregion

    #region 符号
    [Header("Reel Strip")]
    public SymbolType[] reelSymbols;
    private Transform[] symbols;
    private float offsetY = 0f; //很重要，拿来计算整体高度，重新分布位置使用
    private float totalHeight;
    #endregion


    private void Start()
    {
        LoadStripFromChildren();
        GetReelChild();
        NormalizeOffset();
        ApplyPositions();

    }

    private void Update()
    {
        //  1️⃣ Snapping 中禁止一切移动
        if (isSnapping)
            return;

        //不断调整速度以及所有offset 并且回传到ApplyPositions();
        if (isSpinning)
        {
            currentSpeed = speed;

            offsetY += currentSpeed * Time.deltaTime;
            offsetY = Mathf.Repeat(offsetY, totalHeight);

            ApplyPositions();
            return;
        }

        //  正在减速
        if (isStopping)
        {
            //使用moveforward 可以透过TD 来逐渐减速，应为放在update里面的关系
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowSpeed, decel * Time.deltaTime);

            offsetY += currentSpeed * Time.deltaTime;
            offsetY = Mathf.Repeat(offsetY, totalHeight);

            ApplyPositions();

            //这里不能直接使用 currentSpeed == slowspeed , 应为unity 没有办法完全找到完整数
            // ❗ 达到慢速后交给 Snap()
            if (Mathf.Approximately(currentSpeed, slowSpeed))
            {
                isStopping = false;
                StartCoroutine(SnapToNearestSymbol());
            }
        }
    }

    public void StartSpin()
    {
        if (isSnapping || isSpinning || isStopping) return;

        offsetY = Random.Range(0f, totalHeight);
        ApplyPositions();

        currentSpeed = speed;
        isSpinning = true;
    }

    public void StopSpin()
    {
        if (isSnapping || !isSpinning) return;

        isSpinning = false;
        isStopping = true;
    }

    //这是为了让这里的速度可以从slotmachine里面调用
    public void SetSpeedProfile(float spinSpeed, float slow, float deceleration)
    {
        speed = spinSpeed;
        slowSpeed = slow;
        decel = deceleration;
    }


    /// <summary>
    /// 初始让每个symbol 都透过计算好的总高度找到位置，并且丢给offset 然后在start 执行applypos 让他们对齐位置
    /// </summary>
    private void NormalizeOffset()
    {
        float nearest = Mathf.Infinity;
        float nearestY = 0f;

        foreach (var s in symbols)
        {
            float d = Mathf.Abs(s.localPosition.y);
            if (d < nearest)
            {
                nearest = d;
                nearestY = s.localPosition.y;
            }
        }

        offsetY = Mathf.Repeat(nearestY > 0f ? 0f : -nearestY, totalHeight);
    }


    /// <summary>
    /// 面试的时候得让他们知道这里使用GPT 协助，我等级没那么高，但是有理解运作方法
    ///
    /// 作用：
    /// - 计算卷轴内容的中心点（centerBias）
    /// - 根据 offsetY 为每个 symbol 重新计算位置
    /// - 搭配 Mathf.Repeat 实现「无缝循环卷轴」的视觉效果
    /// - 最终更新每个 symbol 的 localPosition
    /// </summary>
    private void ApplyPositions()
    {

        float centerBias = (symbols.Length - 1) * 0.5f * symbolHeight;
        for (int i = 0; i < symbols.Length; i++)
        {
            float baseY = i * symbolHeight;
            float y = Mathf.Repeat(baseY - offsetY, totalHeight) - centerBias; //使用mathF 可以定制他不要超出的范围，否则就是-99999
            Vector3 p = symbols[i].localPosition;
            symbols[i].localPosition = new Vector3(p.x, y, p.z);
        }
    }

    /// <summary>
    /// 这一行参考GPT写给我，由于我没有办法写出那么好的逻辑
    /// 但是已经有经过研究和分析，总结就是 isstopping true ，进入此方法 然后迅速打开 issnapping 让其他会干扰对齐的暂时无法使用
    /// 计算当前中最下面的symbol但是不会超出整体范围，然后透过预设好的duration 一直不断对准
    /// </summary>
    /// <returns></returns>
    private IEnumerator SnapToNearestSymbol()
    {
        isSnapping = true;

        float mod = Mathf.Repeat(offsetY, symbolHeight);
        Debug.Log("mod" + mod);

        float down = mod;
        float up = symbolHeight - mod;
        Debug.Log("up" + up);


        //float target =
        //    (down <= up) ? (offsetY - down) : (offsetY + up);
        //Gpt 教我用3元写法

        float target;
        if (down <= up)
        {
            target = offsetY - down;
        }
        else
        {
            target = offsetY + up;
        }
        //不要让放他超出整体范围
        target = Mathf.Repeat(target, totalHeight);
        Debug.Log("target" + target);
        Debug.Log("totalHeight" + totalHeight);


        // ======== 关键：消除回弹（锁住最终值）========
        float finalTarget = target;
        float duration = 0.12f;
        float elapsed = 0f;

        float start = offsetY;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            //这一行是参考gpt 写给我的，我并没有办法做出这个数学
            //主要的用意就是，回传平滑数值给offsetY 好让applyPosition 对齐
            // 平滑曲线（不会 overshoot）
            t = t * t * (3f - 2f * t);
            offsetY = Mathf.Lerp(start, finalTarget, t);
            ApplyPositions();
            //用时间找到elapsed 相同数值，从而产生慢慢平滑的感觉
            elapsed += Time.deltaTime;
            yield return null;
        }

        //用刚才锁好的 finaltarget 在锁多一次，（不然很容易会出现不平整）（如果速度太快）
        // 最终锁位（0 浮动容忍）
        offsetY = finalTarget;
        ApplyPositions();

        // 关键：锁住 speed，让下一轮不带历史动量
        currentSpeed = speed;

        isSnapping = false;
    }


    /// <summary>
    /// 这里是计算各个symbols 上中下 然后回传去StopReelsSequentially()
    /// 必须使用 mathf。abs 不然的话小数点很容易乱跑
    /// </summary>
    /// <returns></returns>
    public SymbolType[] GetRowResult()
    {
        SymbolType[] results = new SymbolType[3];

        foreach (var s in symbols)
        {
            float y = s.localPosition.y;
            SymbolType type = s.GetComponent<SymbolTag>().type;

            // Middle (最接近 0)
            if (Mathf.Abs(y - 0f) < 0.1f)
                results[1] = type;

            // Top (接近 +1.5)
            else if (Mathf.Abs(y - symbolHeight) < 0.1f)
                results[0] = type;

            // Bottom (接近 -1.5)
            else if (Mathf.Abs(y + symbolHeight) < 0.1f)
                results[2] = type;
        }

        return results;

    }

    /// <summary>
    /// 尋找所有Reel 地下的孩子并且 分配個個高度
    /// </summary>
    public void GetReelChild()
    {
        // 吧挂有这个程式底下的物件数量（child） 加入一个新开的INT n
        int n = transform.childCount;
        //n = 你底下有多少物件
        symbols = new Transform[n];
        //吧每一个孩子存入 symbol 【i】
        for (int i = 0; i < n; i++)
            symbols[i] = transform.GetChild(i);

        //总高度 
        totalHeight = symbolHeight * n;
        currentSpeed = speed;

    }



    /// <summary>
    /// Debug 使用，假設符號忘了加Tag 就會出現
    /// </summary>
    //自动从子物件读取符号列表（完全符合你当前操作方式）
    private void LoadStripFromChildren()
    {
        int n = transform.childCount;
        reelSymbols = new SymbolType[n];

        for (int i = 0; i < n; i++)
        {
            var tag = transform.GetChild(i).GetComponent<SymbolTag>();
            if (tag != null)
                reelSymbols[i] = tag.type;
            else
                Debug.LogWarning($"Child {i} 没有 SymbolTag !");
        }
    }

    public void ApplyWeights(float[] weights)
    {
        List<SymbolType> newStrip = new List<SymbolType>();

        for (int i = 0; i < weights.Length; i++)
        {
            int count = Mathf.FloorToInt(weights[i]);
            for (int x = 0; x < count; x++)
                newStrip.Add((SymbolType)i);
        }

        reelSymbols = newStrip.ToArray();

    }

    public void ForceAlign(SymbolType top, SymbolType mid, SymbolType bottom)
    {
        Transform tTop = null;
        Transform tMid = null;
        Transform tBottom = null;

        float bestTop = Mathf.Infinity;
        float bestMid = Mathf.Infinity;
        float bestBottom = Mathf.Infinity;

        foreach (var s in symbols)
        {
            SymbolType type = s.GetComponent<SymbolTag>().type;
            float y = s.localPosition.y;

            // 找最靠近 1.5 的（上排）
            if (type == top)
            {
                float d = Mathf.Abs(y - symbolHeight);
                if (d < bestTop)
                {
                    bestTop = d;
                    tTop = s;
                }
            }

            // 找最靠近 0 的（中排）
            if (type == mid)
            {
                float d = Mathf.Abs(y - 0f);
                if (d < bestMid)
                {
                    bestMid = d;
                    tMid = s;
                }
            }

            // 找最靠近 -1.5 的（下排）
            if (type == bottom)
            {
                float d = Mathf.Abs(y + symbolHeight);
                if (d < bestBottom)
                {
                    bestBottom = d;
                    tBottom = s;
                }
            }
        }

        // ⚠ 若没找到其中某个 symbol，无法对齐（避免报错）
        if (tTop == null || tMid == null || tBottom == null)
        {
            Debug.LogWarning("ForceAlign 找不到三个目标符号！");
            return;
        }

        // 当前这三个 symbol 的位置
        float yTop = tTop.localPosition.y;
        float yMid = tMid.localPosition.y;
        float yBot = tBottom.localPosition.y;

        // 我们要让它们变成：  top = 1.5 , mid = 0 , bot = -1.5
        float deltaMid = 0f - yMid;

        // 调整 offset，使中间行对齐
        offsetY = Mathf.Repeat(offsetY - deltaMid, totalHeight);

        // 应用新的位置
        ApplyPositions();
    }

#if UNITY_EDITOR
    #region DEBUG

    /// <summary>
    /// Debug 專用，爲了尋找在場的所有符號是否與Tag 相同
    /// </summary>
    /// <returns></returns>
    public SymbolType GetCenterResult()
    {
        Transform center = null;
        float nearest = Mathf.Infinity;

        foreach (var s in symbols)
        {
            float d = Mathf.Abs(s.localPosition.y);
            if (d < nearest)
            {
                nearest = d;
                center = s;
            }
        }

        return center.GetComponent<SymbolTag>().type;
    }

    /// <summary>
    /// 计算当前伤害值
    /// </summary>
    /// <param name="baseDamage">基础伤害</param>
    /// <returns>最终伤害值</returns>
    public int CalculateDamage(int baseDamage)
    {
        return baseDamage * 2;
    }

    
    #endregion
#endif
}