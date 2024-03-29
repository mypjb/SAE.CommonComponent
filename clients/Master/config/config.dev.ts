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
    define: {
        'process.env.UMI_ENV': 'dev'
    }
});