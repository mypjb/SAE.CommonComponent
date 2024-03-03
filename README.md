# SAE.CommonComponent

- ~~文件配置节加入集群标识~~
- ~~调整授权位图的节点名称~~
- ~~初始化的时候赋予超管权限，当前赋值错误！应该使用`SuperAdmin`而不是`Role`~~
- 将移除`RBAC`授权，添加`ABAC`授权人认证

  创建属性标签，标签可以附着再任何上面，比如说`User`、`Certificate`、`RequestFlag`(请求标识)等...

  规则由规则和规则组组成，对`resource`(`endpoint`或者`method`)，的访问要先验证其所属规则。

  ### Data Carrier

  ***

  #### User:

  ```json
  {
    //自身属性
    "age": 18,
    "sex": "man",
    //标签
    "role": "super_admin",
    "duty": "manage",
    "deadline": "2023-10-10",
    ...
  }
  ```

  #### Certificate:

  ```json
  {
    //自身属性
    "appId": 'xxxx-xxx-xxx',
    "scope": 'api',
    //标签
    "dateIssue": "2023-10-10",
    "duration": "30d"
    ...
  }
  ```

  ***

  ### Resource And Rule

  ***

  ### Rule

  ```json
  [
    {
      "id": 1,
      "name": "必须是男性",
      "expression": "sex == 'man'"
    },
    {
      "id": 2,
      "name": "必须成年",
      "expression": "user.age >= 18"
    },
    {
      "id": 3,
      "name": "api域必须是'api'",
      "expression": "cert.scope == 'api'"
    }
  ]
  ```

  ### RuleGroup

  ```json
  [
    {
      "id": 1,
      "name": "成年男性规则",
      "rules": [1 && 2]
    },
    {
      "id": 2,
      "name": "混合规则",
      "rules": [3 || 1 && 2]
    }
  ]
  ```

  #### Endpoint

  ```json
  [
    {
      "id": 1,
      "name": "home",
      "path": "/",
      "ruleGroup": [1] //引用规则组
    },
    {
      "id": 1,
      "name": "api",
      "path": "/api/search",
      "ruleGroup": [1, 2] //引用规则组
    }
  ]
  ```

  #### Method

  ```json
  [
    {
      "id": 1,
      "name": "main",
      "path": "class.home",
      "ruleGroup": [1] //引用规则组
    },
    {
      "id": 1,
      "name": "search",
      "path": "class.find",
      "ruleGroup": [1, 2] //引用规则组
    }
  ]
  ```

---
- 添加注释
- 添加集群测试
- 通用数据添加`appId`标识。
- 解決前端`Application`中`Format.Scope`格式化触发 Hook 异常。
- ~~加入全局集群、应用选项（目前`request`配置在`Master`尚未进行配置）~~
- 将`dict`提取到 Master 公共函数当中
- 权限角色菜单没有加入多租户逻辑，且经过改版后没有应用到前端。
- 角色菜单点击无效。
- 配置（包括权限）加入多租户。
- 初始化的时候，第一个集群标识可以自己指定（仅限开发环境）。
- 生产环境第一个`cluster`、`client`标识由指定生成改为随机生成，同时修改配置文件，并重启系统。
- `client`基于客户端授权时，设置应用权限。
- 加入`label`系统，`label`，标签可以放到任何资源上面。标签类型可以使用字典维护。
- 现在有两个问题：
  
  - 一个集群看成一个租户，多个集群对应多个租户，租户每次开功能时，都在自身的集群当中激活应用。
  - 一个集群看成一条产品线，比如说`电商集群`、`即时通讯集群`、`在线教育集群`等。每个集群都代表各自的产品。

**各系统的多租户策略**

| 系统             | 描述     | 多租户类型 |
| ---------------- | -------- | ---------- |
| Application      | 应用集群 | 数据库     |
| Authorize        | 授权数据 | 表\|数据库 |
| BasicData        | 基础数据 | 数据库     |
| ConfigServer     | 配置数据 | 数据库     |
| MultiTenant      | 多租户   | 数据库     |
| PluginManagement | 插件管理 | 数据库     |
| Routing          | 路由数据 | 表\|数据库 |
| User             | 用户数据 | 数据库     |
