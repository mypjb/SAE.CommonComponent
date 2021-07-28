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
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Headers': '*',
            'Access-Control-Allow-Methods': '*'
        }
    },
    define: {
        'process.env.UMI_ENV': 'dev'
    }
});