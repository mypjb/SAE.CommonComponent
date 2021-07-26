import { useCallback, useEffect, useState } from "react";
import { dict } from "../utils/service";

export default () => {
    const [state, setState] = useState([]);
    const load = useCallback(() => {
        dict.scope()
            .then((data) => {
                setState(data);
            });
    }, []);

    useEffect(async () => {
        load();
    }, []);

    return {
        state,
        load
    }
}