export const dva = {
    config: {
        onError(err) {
            err.preventDefault();
            console.error(err.message);
        },
        initialState: {
            // solution: {
            //     pageIndex: 1,
            //     pageSize: 10,
            //     totalCount: 0,
            //     items: [],
            //     params: {},
            //     model: {}
            // },
        },
    },
};