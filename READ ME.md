# Unity Slot Machine Demo

##  项目简介
这是一个使用 Unity（C#）制作的老虎机（Slot Machine）Demo，
为了面试产生的作品，大概消耗3个月

---

## 核心功能
- 卷轴旋转 
- 8 条 PayLine 赔付线判定
- FreeSpin 系统（不扣钱、自动触发）
- Player Wallet（资金结算与持久化）
- Forced Win（测试 / 展示用）
- RTP 离线统计与验证（Simulation）

---

##  系统结构说明

### MainNumber4 为了启用多项功能
- 作为系统入口
- 串接 Spin → Result → UI → Wallet
- 不负责计算，仅负责流程调度

---

### SlotMachineController（旋转控制）
- 处理 Play Button 输入
- 控制卷轴启动、停轮顺序
- 组装最终 Symbol Matrix
- 将结果交给 MainNumber4

---

### ReelController（卷轴行为）
- 使用 offsetY + Mathf.Repeat 实现无限循环
- 分离 Spinning / Deceleration / Snapping 状态
- Snap 使用平滑曲线避免 overshoot
- 可被 ForcedWin 强制对齐画面

---

### PayLineCheck（赔付线判定）
- 定义 PayLines（row, col）
- 解析最终 Matrix
- 判定中奖并通知 Wallet
- 亮起对应赔付线 UI

---

### FreeSpinCheck（FreeSpin 管理）
- 管理 FreeSpin 数量
- 决定是否扣钱
- FreeSpin 期间自动触发 Spin
- 与 AutoSpin 独立区分

---

### PlayerWallet（资金与 RTP）
- 唯一允许修改玩家金钱的地方
- 统一处理 Bet / Win
- 记录 totalBet / totalWin
- 提供 CurrentRTP 计算
- 使用 PlayerPrefs 持久化 Demo 数据

---

##  RTP 设计说明
- 单局不直接控制 RTP
- 通过累计 totalBet / totalWin 计算 CurrentRTP
- 提供 SimulationTester 进行离线大量模拟验证
- ForcedWin 仅用于测试与展示，不参与正常逻辑

---


## 📎 备注
- 本项目为 Demo 与学习用途
- 部分数学与结构在构思阶段有使用 AI 辅助，
  但所有逻辑均经过理解与调整后整合
- solid 原则 和 mvc 设计模式,本人在努力学习虽然用的不多也非常少

