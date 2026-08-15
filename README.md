# NormalLife

> 🎓 毕业设计 · 基于 Unity 3D 的跑酷游戏，玩法参考《地铁跑酷》(Subway Surfers)

NormalLife 是一款使用 Unity 3D 开发的**三车道无限跑酷**游戏：角色沿道路自动向前奔跑，玩家通过左右切换车道、跳跃和翻滚来躲避障碍物，坚持越久越好。项目中实现了一套完整的状态机角色控制、程序化道路生成与对象池管理。

## ✨ 特性

- 🏃 三车道跑酷（左 / 中 / 右）
- ↔️ 左右切换车道躲避障碍
- ⬆️ 跳跃越过障碍、⬇️ 下蹲翻滚穿过低矮障碍
- 🔄 道路带转弯，角色随路段旋转并自动校正方向
- 🧩 基于 FSM 的角色状态机（idle / run / jump / roll / die）
- 🛣️ 程序化道路生成（ScriptableObject 配置 + 预制体拼接）
- ♻️ 对象池复用道路与障碍物，减少运行时 GC
- 💥 基于碰撞包围盒的撞击部位判定（X / Y / Z 三轴）

## 🎮 操作

| 按键 | 功能 |
| --- | --- |
| `A` / `←` | 向左切换车道 |
| `D` / `→` | 向右切换车道 |
| `W` / `↑` | 跳跃 |
| `S` / `↓` | 下蹲翻滚（同时用于开始奔跑） |
| `S` | 开始奔跑（idle → run） |
| `X` | 回到待机（idle） |

> 屏幕上的 `重开` 按钮可重载当前场景。

## 🛠️ 技术要点

| 模块 | 说明 | 位置 |
| --- | --- | --- |
| 角色控制 | 三车道移动、跳跃、翻滚、转向与方向镜像校正、碰撞判定 | `Assets/NormalLife/Scripts/Character.cs` |
| 角色状态机 | idle / run / jump / roll / die 五状态与迁移事件 | `Assets/NormalLife/Scripts/CharacterFSM.cs` |
| 通用 FSM 框架 | 状态 + 迁移（Translation）的轻量状态机 | `Assets/NormalLife/Scripts/FSM/FSM.cs` |
| 动画控制 | 状态驱动的角色动画切换 | `Assets/NormalLife/Scripts/CharacterAnim.cs` |
| 程序化道路 | GenerateData 配置 + Road 预制体 + 测试场景 | `Assets/NormalLife/GenerateRoad/` |
| 对象池 | 道路 / 障碍物对象复用 | `Assets/NormalLife/Scripts/EasyObjectPool/` |
| 补间动画 | DOTween 驱动过渡动画 | 依赖插件（`DG.Tweening`） |
| 着色器 | 自定义 Shader | `Assets/NormalLife/Shaders/` |

> 说明：`CharacterFSM.cs` 中二段跳（double jump）状态与迁移逻辑已预留但默认注释关闭。

## 📁 项目结构

```
NormalLifeProject/Assets/
├── NormalLife/          # 游戏主体
│   ├── Scripts/         # 角色、FSM、对象池、工具脚本
│   │   ├── Character.cs
│   │   ├── CharacterFSM.cs
│   │   ├── CharacterAnim.cs
│   │   ├── FSM/         # 通用状态机框架
│   │   ├── EasyObjectPool/
│   │   └── Utils/
│   ├── GenerateRoad/    # 程序化道路生成（配置 / 预制体 / 脚本）
│   ├── Scenes/          # OriginModel（主场景）、RoadTest（道路测试）
│   ├── Shaders/         # 自定义着色器
│   ├── Resources/       # 运行时加载资源
│   └── Editor/          # 编辑器扩展
├── SubwaySurfers/       # 角色与场景美术资源
├── Plugins/             # 第三方插件
└── Third/               # 第三方资源
```

## 🚀 运行

### 环境要求

- **Unity 2019.4.8f1**（建议用 Unity Hub 安装对应版本）
- **DOTween**（动画补间，项目已依赖 `DG.Tweening`）

### 步骤

1. 使用 Unity Hub 打开 `NormalLifeProject` 目录
2. 打开场景 `Assets/NormalLife/Scenes/OriginModel.unity`
3. 点击 Play，按 `S` 开始奔跑

## 📷 截图

<!-- TODO: 补充运行截图 -->

## 📄 许可

仅用于学习交流与毕业设计展示。
