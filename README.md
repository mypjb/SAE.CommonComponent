# SAE.CommonComponent

- ~~文件配置节加入集群标识~~
- ~~调整授权位图的节点名称~~
- ~~初始化的时候赋予超管权限，当前赋值错误！应该使用`SuperAdmin`而不是`Role`~~
- 通用数据添加`appId`标识。
- 解決前端`Application`中`Format.Scope`格式化触发Hook异常。
- ~~加入全局集群、应用选项（目前`request`配置在`Master`尚未进行配置）~~
- 将`dict`提取到Master公共函数当中
- 权限角色菜单没有加入多租户逻辑，且经过改版后没有应用到前端。
- 角色菜单点击无效。
- 配置（包括权限）加入多租户。
- 初始化的时候，第一个集群标识可以自己指定（仅限开发环境）。
- 生产环境第一个`cluster`、`client`标识由指定生成改为随机生成，同时修改配置文件，并重启系统。
- `client`基于客户端授权时，设置应用权限。



__各系统的多租户策略__

| 系统 | 描述 | 多租户类型 |
| --- | --- | --- |
| Application | 应用集群 | 数据库 |
| Authorize | 授权数据 | 表\|数据库 |
| BasicData | 基础数据 | 数据库 |
| ConfigServer | 配置数据 | 数据库 |
| MultiTenant | 多租户 | 数据库 |
| PluginManagement | 插件管理 | 数据库 |
| Routing | 路由数据 | 表\|数据库 |
| User | 用户数据 | 数据库 |
