const ENV = process.env.NODE_ENV;

console.log({ENV,UMI_ENV:process.env.UMI_ENV});
let apps = [
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
    },
    {
        name: 'authorize', // 唯一 id
        entry: '//dev.sae.com:8005', // html entry
        path:"/auth",
    }
];

if (ENV != "development") {
    apps[0].entry='//configserver.client.sae.com'
    apps[1].entry='//identity.client.sae.com'
    apps[2].entry='//oauth.client.sae.com'
    apps[3].entry='//routing.client.sae.com'
}
console.log(apps);
export default apps;
