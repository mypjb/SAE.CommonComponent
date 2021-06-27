import { defineConfig } from 'umi';
export default defineConfig({
    nodeModulesTransform: { type: 'none', exclude: [] },
    targets: {
        chrome: 79,
        firefox: false,
        safari: false,
        edge: false,
        ios: false,
    },
    devServer: {
        headers: {
            "Access-Control-Allow-Credentials": true,
            "Access-Control-Allow-Headers": "*",
            "Access-Control-Allow-Methods": "*",
            "Access-Control-Allow-Origin": "*"
        }
    },
    define: {
        'process.env.UMI_ENV': 'dev'
    }
});