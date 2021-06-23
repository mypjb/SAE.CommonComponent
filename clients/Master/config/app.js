const ENV = process.env.NODE_ENV;

let apps = [];

if (ENV == "development") {
    apps = [
        {
            name: 'config-server', // 唯一 id
            entry: '//dev.sae.com:8001', // html entry
            path:"/config",
        },
        {
            name: 'identity', // 唯一 id
            entry: '//dev.sae.com:8002', // html entry
            path:"/identity",

        },
        {
            name: 'oauth', // 唯一 id
            entry: '//dev.sae.com:8003', // html entry
            path:"/oauth",

        },
        {
            name: 'routing', // 唯一 id
            entry: '//dev.sae.com:8004', // html entry
            path:"/routing",
        }
    ];
} else {
    apps = [
        {
            name: 'config-server', // 唯一 id
            entry: '//configserver.client.sae.com', // html entry
        },
        {
            name: 'identity', // 唯一 id
            entry: '//identity.client.sae.com', // html entry
        },
        {
            name: 'oauth', // 唯一 id
            entry: '//oauth.client.sae.com', // html entry
        },
        {
            name: 'routing', // 唯一 id
            entry: '//routing.client.sae.com', // html entry
        }
    ];
}

export default apps;