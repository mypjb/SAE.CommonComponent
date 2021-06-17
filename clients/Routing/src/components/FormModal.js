import { Modal } from "antd"
import { useModel } from "umi";

//build options
const buildOptions = (props, model) => {
    const handler = {
        onOk: (e) => { console.warn("Use default onOk"); },
        onCancel: (e) => { console.warn("Use default onCancel"); },
    };
    const okCallback = function (fn) {
        if (fn.onOk && fn.onCancel) {
            handler.onOk = fn;
            handler.onCancel = fn.onCancel;
        } else {
            handler.onOk = fn;
        }
    };

    const options = {
        ...props,
        onOk: (e) => {
            if (props.onOk) {
                props.onOk(e);
            }
            if (handler.onOk(e) !== false) {
                e();
            }
        },
        onCancel: (e) => {
            let result = true;
            if (props.onCancel) {
                props.onCancel(e);
            }
            result = handler.onCancel(e);
            if (result !== false) {
                e();
            }
        }
    };

    if (props.contentElement) {
        const contentProps = {
            okCallback,
            closeCallback: (data) => {
                model.destroy();
                if (props.okCallback) {
                    props.okCallback(data);
                }
            },
            ...props.contentProps,
        };
        options.content = (<props.contentElement {...contentProps}></props.contentElement>);
    }

    return options;
};

const FormModal = function (props) {
    const [modal, contextHolder] = Modal.useModal();
    const options = buildOptions(props, modal);
    modal.confirm(options);
    return contextHolder;
}

FormModal.confirm = function (props, proxyModal) {
    if (proxyModal) {
        let model;
        if (props.contentElement) {
            props.contentProps = props.contentProps || {};
            if (!props.contentProps.closeCallback) {
                {
                    props.contentProps.closeCallback = () => {
                        model.destroy();
                        if (props.onOk) {
                            props.onOk();
                        }
                    }
                }
            }
        }
        const options = buildOptions(props, proxyModal);
        model = proxyModal.confirm(options);
    } else {
        const model = Modal.confirm();
        const options = buildOptions(props, model);
        model.update(options);
    }
};

export default FormModal;