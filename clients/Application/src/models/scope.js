import { useCallback, useEffect, useState } from "react";
import { dict } from "../utils/service";

export default () => {
    const [state, setState] = useState([]);
    const load = useCallback(() => {
        dict.scope()
            .then((response) => {
                console.log(response);
                setState(response.data);
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